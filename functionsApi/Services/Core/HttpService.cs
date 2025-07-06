
using System.Net.Http.Headers;

namespace Services.HttpService
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _client = new HttpClient();
        public async Task<HttpResponseMessage> MakeGetRequest(string url, string token, string tokenType = "Bearer")
        {
            Console.WriteLine($"HttpService: Making GET request to {url}");
            Console.WriteLine($"HttpService: Using {tokenType} authorization");
            
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            
            // Add required headers for Supabase
            if (url.Contains("supabase.co"))
            {
                request.Headers.Add("apikey", token);
                request.Headers.Add("Authorization", $"Bearer {token}");
            }
            else
            {
                request.Headers.Add("Authorization", $"{tokenType} {token}");
            }

            try
            {
                var response = await _client.SendAsync(request);
                Console.WriteLine($"HttpService: Response status: {response.StatusCode}");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HttpService: Exception occurred: {ex.Message}");
                throw;
            }
        }
        public async Task<HttpResponseMessage> MakePostRequest(string url, string token, HttpContent content, string tokenType = "Bearer")
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", $"{tokenType} {token}");
            request.Content = content;

            var response = await _client.SendAsync(request);
            return response;
        }

        public async Task<HttpResponseMessage> MakePutRequest(string url, string token, HttpContent content, string contentType)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, url);
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _client.SendAsync(request);
            return response;
        }
    }
}