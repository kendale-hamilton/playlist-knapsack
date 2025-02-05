
namespace Services.HttpService
{
    public interface IHttpService
    {
        Task<HttpResponseMessage> MakeGetRequest(string url, string token);
    }
}