using System.Net;

namespace SwissLife.Slkv.Partner.ClientAppSample.Models
{
    public class ApiResponse<T> : ApiResponse
    {
        public ApiResponse(T successData)
            : base()
        {
            SuccessData = successData;
        }

        public ApiResponse(HttpStatusCode failureStatusCode)
            : base(failureStatusCode)
        {
        }

        public T SuccessData { get; }
    }
}