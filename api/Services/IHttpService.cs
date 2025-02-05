using Models.Knapsack;

namespace Services
{
    public interface IHttpService
    {
        Task<HttpResponseMessage> MakeGetRequest(string url, string token);
    }
}