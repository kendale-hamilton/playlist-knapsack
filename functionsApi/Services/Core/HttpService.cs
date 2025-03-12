
using System.Net.Http.Headers;

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
        public async Task<HttpResponseMessage> MakePostRequest(string url, string token, HttpContent content)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", $"Bearer {token}");
            request.Content = content;

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return response;
        }

        public async Task<HttpResponseMessage> MakePutRequest(string url, string token, HttpContent content, string contentType)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, url);
            request.Headers.Add("Authorization", $"Bearer {token}");
            Console.WriteLine("Request: " + request.ToString());
            Console.WriteLine("Content: " + content.ReadAsStringAsync().Result);

            var response = await _client.SendAsync(request);
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}