using System.Net;

namespace SwissLife.Slkv.Partner.ClientAppSample.Models
{
    public class ApiResponse
    {
        public ApiResponse()
        {
            Successful = true;
        }

        public ApiResponse(HttpStatusCode failureStatusCode)
        {
            FailureStatusCode = failureStatusCode;
            Successful = false;
        }

        public HttpStatusCode FailureStatusCode { get; }

        public bool Successful { get; }
    }
}