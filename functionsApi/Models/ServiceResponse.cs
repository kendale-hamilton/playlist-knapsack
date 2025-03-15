using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Models.ServiceResponse
{
    public class ServiceResponse<T>
    {
        public required HttpStatusCode Status { get; set; }
        public T? Data { get; set; }
        public string? ErrorMessage { get; set; }
        public IActionResult ToActionResult()
        {
            if (Status == HttpStatusCode.OK)
            {
                return new OkObjectResult(Data);
            }
            else if (Status == HttpStatusCode.BadRequest)
            {
                return new BadRequestObjectResult(ErrorMessage);
            }
            else if (Status == HttpStatusCode.Unauthorized)
            {
                return new UnauthorizedResult();
            }
            else if (Status == HttpStatusCode.NotFound)
            {
                return new NotFoundObjectResult(ErrorMessage);
            }
            else if (Status == HttpStatusCode.InternalServerError)
            {
                return new ObjectResult(ErrorMessage)
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
            else
            {
                return new ObjectResult(ErrorMessage)
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}