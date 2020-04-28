using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SwissLife.Slkv.Partner.ClientAppSample.Models;

namespace SwissLife.Slkv.Partner.ClientAppSample.Services
{
    public class SlkvPartnerApiClient : ISlkvPartnerApiClient
    {
        private readonly string endpoint;
        private readonly IHttpClientFactory httpClientFactory;

        public SlkvPartnerApiClient(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            this.endpoint = configuration["SlkvPartnerApi:Endpoint"];
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<ApiResponse<ContractsModel>> GetContractsAsync(
            string accessToken,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await SendApiRequestAsync(
                accessToken,
                "/contract",
                HttpMethod.Get,
                cancellationToken: cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<ContractsModel>(response.StatusCode);
            }

            string json = await response.Content.ReadAsStringAsync();
            ContractsModel model = JsonConvert.DeserializeObject<ContractsModel>(json);
            return new ApiResponse<ContractsModel>(model);
        }

        public async Task<ApiResponse<InsuredPersonsModel>> GetInsuredPersonsAsync(
            string accessToken,
            string contractId,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await SendApiRequestAsync(
                accessToken,
                $"/contract/{contractId}/insured-person",
                HttpMethod.Get,
                cancellationToken: cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<InsuredPersonsModel>(response.StatusCode);
            }

            string json = await response.Content.ReadAsStringAsync();
            InsuredPersonsModel model = JsonConvert.DeserializeObject<InsuredPersonsModel>(json);
            foreach (InsuredPersonModel insuredPerson in model.InsuredPersons)
            {
                insuredPerson.ContractId = contractId;
            }

            return new ApiResponse<InsuredPersonsModel>(model);
        }

        public async Task<ApiResponse<ValidationResultModel>> ValidateChangeSalaryAsync(
            string accessToken,
            InsuredPersonModel insuredPersonModel,
            CancellationToken cancellationToken)
        {
            if (insuredPersonModel == null)
            {
                throw new ArgumentNullException(nameof(insuredPersonModel));
            }

            HttpResponseMessage response = await SendApiRequestAsync(
                accessToken,
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
            return new ApiResponse<ValidationResultModel>(validationResultModel);
        }

        public async Task<ApiResponse> StartChangeSalaryAsync(
            string accessToken,
            InsuredPersonModel insuredPersonModel,
            CancellationToken cancellationToken)
        {
            if (insuredPersonModel == null)
            {
                throw new ArgumentNullException(nameof(insuredPersonModel));
            }

            HttpResponseMessage response = await SendApiRequestAsync(
                accessToken,
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
                return new ApiResponse();
            }

            return new ApiResponse(response.StatusCode);
        }

        public async Task<ApiResponse<DocumentsModel>> GetDocumentsAsync(
            string accessToken,
            string refCorrelationId,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await SendApiRequestAsync(
                accessToken,
                $"/document?refCorrelationId={refCorrelationId}",
                HttpMethod.Get,
                cancellationToken: cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<DocumentsModel>(response.StatusCode);
            }

            string json = await response.Content.ReadAsStringAsync();
            DocumentsModel model = JsonConvert.DeserializeObject<DocumentsModel>(json);
            return new ApiResponse<DocumentsModel>(model);
        }

        public async Task<ApiResponse<DocumentContentModes>> DownloadDocumentAsync(
            string accessToken,
            string documentId,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await SendApiRequestAsync(
                accessToken,
                $"/document/{documentId}/content",
                HttpMethod.Get,
                cancellationToken: cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<DocumentContentModes>(response.StatusCode);
            }

            byte[] content = await response.Content.ReadAsByteArrayAsync();
            return new ApiResponse<DocumentContentModes>(new DocumentContentModes
            {
                Content = content,
                ContentType = response.Content.Headers.ContentType.ToString(),
                FileName = response.Content.Headers.ContentDisposition.FileName
            });
        }

        private async Task<HttpResponseMessage> SendApiRequestAsync(
            string accessToken,
            string path,
            HttpMethod method,
            object data = null,
            CancellationToken cancellationToken = default)
        {
            HttpClient httpClient = httpClientFactory.CreateClient();
            using (HttpRequestMessage request = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri($"https://{endpoint}{path}"),
            })
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                if (data != null)
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");
                }

                return await httpClient.SendAsync(request, cancellationToken);
            }
        }
    }
}
