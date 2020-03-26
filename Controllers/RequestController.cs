using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SwissLife.Slkv.Partner.ClientAppSample.Models;

namespace SwissLife.Slkv.Partner.ClientAppSample.Controllers
{
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

        public async Task<IActionResult> Contracts()
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            HttpClient httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await httpClient.GetAsync($"https://{configuration["SlkvPartnerApi:Endpoint"]}/contract");
            string json = await response.Content.ReadAsStringAsync();
            ContractsModel model = JsonConvert.DeserializeObject<ContractsModel>(json);
            return View(model);
        }

        public async Task<IActionResult> InsuredPersons(string contractId = "B01482DD9BED4927BEB5BB54EFD6D6E2")
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            HttpClient httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await httpClient.GetAsync($"https://{configuration["SlkvPartnerApi:Endpoint"]}/contract/{contractId}/insured-person");
            string json = await response.Content.ReadAsStringAsync();
            InsuredPersonsModel model = JsonConvert.DeserializeObject<InsuredPersonsModel>(json);
            return View(model);
        }
    }
}