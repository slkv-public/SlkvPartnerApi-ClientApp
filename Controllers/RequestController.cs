using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
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

        public async Task<IActionResult> Contracts(
            CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response = await SendApiRequestAsync(
                "/contract", 
                HttpMethod.Get,
                cancellationToken: cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return ViewError(response);
            }

            string json = await response.Content.ReadAsStringAsync();
            ContractsModel model = JsonConvert.DeserializeObject<ContractsModel>(json);
            return View(model);
        }

        public async Task<IActionResult> InsuredPersons(
            string contractId,
            CancellationToken cancellationToken = default)
        {
            contractId ??= configuration["SampleData:ContractId"];

            HttpResponseMessage response = await SendApiRequestAsync(
                $"/contract/{contractId}/insured-person", 
                HttpMethod.Get,
                cancellationToken: cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return ViewError(response);
            }

            string json = await response.Content.ReadAsStringAsync();
            InsuredPersonsModel model = JsonConvert.DeserializeObject<InsuredPersonsModel>(json);
            foreach (InsuredPersonModel insuredPerson in model.InsuredPersons)
            {
                insuredPerson.ContractId = contractId;
            }

            return View(model);
        }

        public IActionResult ChangeSalaryForm(
            InsuredPersonModel insuredPersonModel)
        {
            return View(insuredPersonModel);
        }

        public async Task<IActionResult> ChangeSalary(
            InsuredPersonModel insuredPersonModel,
            CancellationToken cancellationToken = default)
        {
            // we could execute a validation before sending the request.
            if (!await ValidateChangeSalary(insuredPersonModel))
            {
                return View("ChangeSalaryForm", insuredPersonModel);
            }

            // okay, now we send the request to start the business process.
            HttpResponseMessage response = await SendApiRequestAsync(
                $"/contract/{insuredPersonModel.ContractId}/insured-person/{insuredPersonModel.InsuredPersonId}/salary",
                HttpMethod.Post,
                new
                {
                    effectiveAnnualSalary = insuredPersonModel.AnnualSalary,
                    employmentRate = insuredPersonModel.EmploymentRate,
                    dueDate = DateTime.Now,
                },
                cancellationToken: cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return View();
            }

            return ViewError(response);
        }

        public async Task<IActionResult> Documents(
            string refCorrelationId,
            CancellationToken cancellationToken = default)
        {
            refCorrelationId ??= configuration["SampleData:RefCorrelationId"];

            HttpResponseMessage response = await SendApiRequestAsync(
                $"/document?refCorrelationId={refCorrelationId}", 
                HttpMethod.Get,
                cancellationToken: cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return ViewError(response);
            }

            string json = await response.Content.ReadAsStringAsync();
            DocumentsModel model = JsonConvert.DeserializeObject<DocumentsModel>(json);
            return View(model);
        }

        public async Task<IActionResult> DownloadDocument(
            string documentId,
            CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response = await SendApiRequestAsync(
                $"/document/{documentId}/content", 
                HttpMethod.Get,
                cancellationToken: cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return ViewError(response);
            }

            byte[] content = await response.Content.ReadAsByteArrayAsync();
            return File(content, response.Content.Headers.ContentType.ToString(), response.Content.Headers.ContentDisposition.FileName);
        }

        private async Task<bool> ValidateChangeSalary(
            InsuredPersonModel insuredPersonModel,
            CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response = await SendApiRequestAsync(
                $"/contract/{insuredPersonModel.ContractId}/insured-person/{insuredPersonModel.InsuredPersonId}/salary/validation",
                HttpMethod.Post,
                new
                {
                    effectiveAnnualSalary = insuredPersonModel.AnnualSalary,
                    employmentRate = insuredPersonModel.EmploymentRate,
                    dueDate = DateTime.Now,
                },
                cancellationToken: cancellationToken);

            string validationResponseContent = await response.Content.ReadAsStringAsync();
            ValidationResultModel validationResultModel = JsonConvert.DeserializeObject<ValidationResultModel>(validationResponseContent);
            if (validationResultModel.IsValid)
            {
                return true;
            }

            foreach (ValidationMemberModel member in validationResultModel.Members)
            {
                ModelState.AddModelError(member.Member, string.Join(", ", member.Errors.Select(error => error.Code)));
            }

            return false;
        }

        private async Task<HttpResponseMessage> SendApiRequestAsync(
            string path, 
            HttpMethod method,
            object data = null,
            CancellationToken cancellationToken = default)
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

            return await httpClient.SendAsync(request, cancellationToken);
        }

        private IActionResult ViewError(HttpResponseMessage response)
        {
            ViewBag.StatusCode = response.StatusCode;
            return View("Error");
        }
    }
}