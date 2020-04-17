using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SwissLife.Slkv.Partner.ClientAppSample.Models;

namespace SwissLife.Slkv.Partner.ClientAppSample.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;

        public RequestController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }

        public Task<IActionResult> Contracts()
        {
            return RequestApiJsonAsync<ContractsModel>("/contract"); ;
        }

        public Task<IActionResult> InsuredPersons(string contractId = "B01482DD9BED4927BEB5BB54EFD6D6E2")
        {
            return RequestApiAsync(
                $"/contract/{contractId}/insured-person",
                async response => {
                    string json = await response.Content.ReadAsStringAsync();
                    InsuredPersonsModel model = JsonConvert.DeserializeObject<InsuredPersonsModel>(json);
                    foreach (InsuredPersonModel insuredPerson in model.InsuredPersons)
                    {
                        insuredPerson.ContractId = contractId;
                    }

                    return View(model);
                });
        }

        public IActionResult ChangeSalaryForm(InsuredPersonModel insuredPersonModel)
        {
            return View(insuredPersonModel);
        }

        public async Task<IActionResult> ChangeSalary(InsuredPersonModel insuredPersonModel)
        {
            HttpResponseMessage response = await SendApiRequestAsync(
                $"/contract/{insuredPersonModel.ContractId}/insured-person/{insuredPersonModel.InsuredPersonId}/salary",
                HttpMethod.Post,
                new
                {
                    effectiveAnnualSalary = insuredPersonModel.AnnualSalary,
                    employmentRate = insuredPersonModel.EmploymentRate,
                    dueDate = DateTime.Now,
                });
            if (response.IsSuccessStatusCode)
            {
                return View();
            }

            return StatusCode((int)response.StatusCode);
        }

        public Task<IActionResult> Documents(string refCorrelationId = "3921DDC2-3587-4B65-91F0-D30278C8FB5A")
        {
            return RequestApiJsonAsync<DocumentsModel>($"/document?refCorrelationId={refCorrelationId}");
        }

        public Task<IActionResult> DownloadDocument(string documentId)
        {
            return RequestApiAsync(
                $"/document/{documentId}/content",
                async response => {
                    byte[] content = await response.Content.ReadAsByteArrayAsync();
                    return File(content, response.Content.Headers.ContentType.ToString(), response.Content.Headers.ContentDisposition.FileName);
                });
        }

        private Task<IActionResult> RequestApiJsonAsync<T>(string path)
        {
            return RequestApiAsync(
                path,
                async response => {
                    string json = await response.Content.ReadAsStringAsync();
                    T model = JsonConvert.DeserializeObject<T>(json);
                    return View(model);
                });
        }

        private async Task<IActionResult> RequestApiAsync(string path, Func<HttpResponseMessage, Task<IActionResult>> handler)
        {
            HttpResponseMessage response = await SendApiRequestAsync(path, HttpMethod.Get);
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            return await handler.Invoke(response);
        }

        private async Task<HttpResponseMessage> SendApiRequestAsync(
            string path, 
            HttpMethod method,
            object data = null)
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            HttpClient httpClient = httpClientFactory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri($"https://{configuration["SlkvPartnerApi:Endpoint"]}{path}"),
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            if (data != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");
            }

            return await httpClient.SendAsync(request);
        }
    }
}