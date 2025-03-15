
namespace Services.HttpService
{
    public interface IHttpService
    {
        Task<HttpResponseMessage> MakeGetRequest(string url, string token);
        Task<HttpResponseMessage> MakePostRequest(string url, string token, HttpContent content, string tokenType = "Bearer");
        Task<HttpResponseMessage> MakePutRequest(string url, string token, HttpContent content, string contentType);
    }
}