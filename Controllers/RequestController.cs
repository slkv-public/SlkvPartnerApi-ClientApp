using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SwissLife.Slkv.Partner.ClientAppSample.Models;
using SwissLife.Slkv.Partner.ClientAppSample.Services;

namespace SwissLife.Slkv.Partner.ClientAppSample.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ISlkvPartnerApiClient slkvPartnerApiClient;

        public RequestController(
            IConfiguration configuration,
            ISlkvPartnerApiClient slkvPartnerApiClient)
        {
            this.configuration = configuration;
            this.slkvPartnerApiClient = slkvPartnerApiClient;
        }

        public async Task<IActionResult> Contracts(
            CancellationToken cancellationToken = default)
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            ApiResponse<ContractsModel> result = await slkvPartnerApiClient.GetContractsAsync(
                accessToken,
                cancellationToken);
            if (!result.Successful)
            {
                return ViewError(result.FailureStatusCode);
            }

            return View(result.SuccessData);
        }

        public async Task<IActionResult> InsuredPersons(
            string contractId,
            CancellationToken cancellationToken = default)
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            ApiResponse<InsuredPersonsModel> result = await slkvPartnerApiClient.GetInsuredPersonsAsync(
                accessToken,
                contractId ?? configuration["SampleData:ContractId"],
                cancellationToken);
            if (!result.Successful)
            {
                return ViewError(result.FailureStatusCode);
            }

            return View(result.SuccessData);
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
            string accessToken = await HttpContext.GetTokenAsync("access_token");

            // we could execute a validation before sending the request.
            ApiResponse<ValidationResultModel> validationResult = await slkvPartnerApiClient.ValidateChangeSalaryAsync(
                accessToken,
                insuredPersonModel,
                cancellationToken);
            if (!validationResult.SuccessData.IsValid)
            {
                foreach (ValidationMemberModel member in validationResult.SuccessData.Members)
                {
                    ModelState.AddModelError(member.Member, string.Join(", ", member.Errors.Select(error => error.Code)));
                }

                return View("ChangeSalaryForm", insuredPersonModel);
            }

            // okay, now we send the request to start the business process.
            ApiResponse executionResult = await slkvPartnerApiClient.StartChangeSalaryAsync(
                accessToken,
                insuredPersonModel,
                cancellationToken);
            if (executionResult.Successful)
            {
                return View();
            }

            return ViewError(executionResult.FailureStatusCode);
        }

        public async Task<IActionResult> Documents(
            string refCorrelationId,
            CancellationToken cancellationToken = default)
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            ApiResponse<DocumentsModel> result = await slkvPartnerApiClient.GetDocumentsAsync(
                accessToken,
                refCorrelationId ?? configuration["SampleData:RefCorrelationId"],
                cancellationToken);
            if (!result.Successful)
            {
                return ViewError(result.FailureStatusCode);
            }

            return View(result.SuccessData);
        }

        public async Task<IActionResult> DownloadDocument(
            string documentId,
            CancellationToken cancellationToken = default)
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            ApiResponse<DocumentContentModes> result = await slkvPartnerApiClient.DownloadDocumentAsync(
                accessToken,
                documentId,
                cancellationToken);
            if (!result.Successful)
            {
                return ViewError(result.FailureStatusCode);
            }

            return File(
                result.SuccessData.Content,
                result.SuccessData.ContentType,
                result.SuccessData.FileName);
        }

        private IActionResult ViewError(HttpStatusCode statusCode)
        {
            ViewBag.StatusCode = statusCode;
            return View("Error");
        }
    }
}