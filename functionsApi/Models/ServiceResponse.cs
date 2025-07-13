using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Models.ServiceResponse
{
    public class ServiceResponse
    {
        public HttpStatusCode Status { get; set; }
        public string? ErrorMessage { get; set; }
        public static ServiceResponse Error(HttpStatusCode status, string errorMessage)
        {
            return new ServiceResponse
            {
                Status = status,
                ErrorMessage = errorMessage
            };
        }
        public static IActionResult ToIActionResult<T>(ServiceResponse<T> response)
        {
            if (response.Status == HttpStatusCode.OK)
            {
                return new OkObjectResult(response.Data);
            }
            else
            {
                return new ObjectResult(response.ErrorMessage)
                {
                    StatusCode = (int)response.Status
                };
            }
        }
    }
    public class ServiceResponse<T> : ServiceResponse
    {
        public T? Data { get; set; }
        public ServiceResponse<T> Success(T data)
        {
            return new ServiceResponse<T>
            {
                Status = HttpStatusCode.OK,
                Data = data
            };
        }

        public new static ServiceResponse<T> Error(HttpStatusCode status, string errorMessage)
        {
            return new ServiceResponse<T>
            {
                Status = status,
                ErrorMessage = errorMessage
            };
        }
    }
}