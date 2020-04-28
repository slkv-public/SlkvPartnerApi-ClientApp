using System.Threading;
using System.Threading.Tasks;
using SwissLife.Slkv.Partner.ClientAppSample.Models;

namespace SwissLife.Slkv.Partner.ClientAppSample.Services
{
    public interface ISlkvPartnerApiClient
    {
        Task<ApiResponse<ContractsModel>> GetContractsAsync(
            string accessToken,
            CancellationToken cancellationToken);

        Task<ApiResponse<InsuredPersonsModel>> GetInsuredPersonsAsync(
            string accessToken,
            string contractId,
            CancellationToken cancellationToken);

        Task<ApiResponse<DocumentsModel>> GetDocumentsAsync(
            string accessToken,
            string refCorrelationId,
            CancellationToken cancellationToken);

        Task<ApiResponse<DocumentContentModes>> DownloadDocumentAsync(
            string accessToken,
            string documentId,
            CancellationToken cancellationToken);

        Task<ApiResponse<ValidationResultModel>> ValidateChangeSalaryAsync(
            string accessToken,
            InsuredPersonModel insuredPersonModel,
            CancellationToken cancellationToken);

        Task<ApiResponse> StartChangeSalaryAsync(
            string accessToken,
            InsuredPersonModel insuredPersonModel,
            CancellationToken cancellationToken);
    }
}
