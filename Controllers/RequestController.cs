using System;
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
            return RequestApiJsonAsync<InsuredPersonsModel>($"/contract/{contractId}/insured-person");
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

        private Task<IActionResult> RequestApiJsonAsync<T>(string method)
        {
            return RequestApiAsync(
                method,
                async response => {
                    string json = await response.Content.ReadAsStringAsync();
                    T model = JsonConvert.DeserializeObject<T>(json);
                    return View(model);
                });
        }

        private async Task<IActionResult> RequestApiAsync(string method, Func<HttpResponseMessage, Task<IActionResult>> handler)
        {
            string accessToken = await GetAccessTokenAsync();
            HttpClient httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await httpClient.GetAsync($"https://{configuration["SlkvPartnerApi:Endpoint"]}{method}");
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            return await handler.Invoke(response);
        }

        private async Task<string> GetAccessTokenAsync()
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            // Store the token for current user for reuse and renewal.
            return accessToken;
        }
    }
}