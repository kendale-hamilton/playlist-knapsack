
namespace Services.HttpService
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _client = new HttpClient();
        public async Task<HttpResponseMessage> MakeGetRequest(string url, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}