
using System.Net;
using Models.ServiceResponse;
using Supabase.Postgrest;
using Supabase.Postgrest.Models;

namespace Services.Base
{
    public class ServiceBase
    {
        public readonly Supabase.Client _supabaseClient;
        private readonly HttpClient _httpClient;
        public ServiceBase()
        {
            var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL") ?? "";
            var supabaseServiceKey = Environment.GetEnvironmentVariable("SUPABASE_SERVICE_ROLE_KEY") ?? "";
            if (string.IsNullOrEmpty(supabaseUrl))
            {
                Console.WriteLine("ERROR: SUPABASE_URL environment variable is not set!");
                throw new InvalidOperationException("SUPABASE_URL environment variable is not set");
            }
            if (string.IsNullOrEmpty(supabaseServiceKey))
            {
                Console.WriteLine("ERROR: SUPABASE_SERVICE_ROLE_KEY environment variable is not set!");
                throw new InvalidOperationException("SUPABASE_SERVICE_ROLE_KEY environment variable is not set");
            }
            
            var options = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = false
            };
            
            _supabaseClient = new Supabase.Client(supabaseUrl, supabaseServiceKey, options);
            _httpClient = new HttpClient();
        }

        public async Task<HttpResponseMessage> MakeGetRequest(string url, string token, string tokenType = "Bearer")
        {
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
                var response = await _httpClient.SendAsync(request);
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

            var response = await _httpClient.SendAsync(request);
            return response;
        }

        public async Task<HttpResponseMessage> MakePutRequest(string url, string token, HttpContent content, string contentType)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, url);
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _httpClient.SendAsync(request);
            return response;
        }

        public async Task<HttpResponseMessage> MakeDeleteRequest(string url, string token, string tokenType = "Bearer")
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, url);
            request.Headers.Add("Authorization", $"{tokenType} {token}");
            var response = await _httpClient.SendAsync(request);
            return response;
        }

        public async Task<ServiceResponse<List<T>>> GetEntities<T>(List<string>? ids = null, string? columnName = null) where T : BaseModel, new()
        {
            try
            {
                var query = _supabaseClient.From<T>().Select("*");

                if (ids != null && ids.Count > 0)
                {
                    var validIds = ids.Where(id => !string.IsNullOrEmpty(id)).ToList();
                    query = query.Filter(columnName ?? "id", Constants.Operator.In, validIds);
                }

                var response = await query.Get();
                
                if (response.Models == null || response.Models.Count == 0)
                {
                    return new ServiceResponse<List<T>>
                    {
                        Status = HttpStatusCode.NotFound,
                        ErrorMessage = "Entity not found"
                    };
                }

                return new ServiceResponse<List<T>>
                {
                    Status = HttpStatusCode.OK,
                    Data = response.Models
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<T>>
                {
                    Status = HttpStatusCode.InternalServerError,
                    ErrorMessage = $"Error fetching entity: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<T>> CreateEntity<T>(T entity) where T : BaseModel, new()
        {
            try
            {
                var response = await _supabaseClient.From<T>().Insert(entity);
                return new ServiceResponse<T> { Status = HttpStatusCode.OK, Data = response.Models.First() };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<T> { Status = HttpStatusCode.InternalServerError, ErrorMessage = $"Error creating entity: {ex.Message}" };
            }
        }

        public async Task<ServiceResponse<T>> UpdateEntity<T>(T entity) where T : BaseModel, new()
        {
            try
            {
                var response = await _supabaseClient.From<T>().Update(entity);
                if (response.Content == null)
                {
                    return new ServiceResponse<T>
                    {
                        Status = HttpStatusCode.NotFound,
                        ErrorMessage = "Entity not found"
                    };
                }
                return new ServiceResponse<T> { Status = HttpStatusCode.OK, Data = response.Models.First() };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<T> { Status = HttpStatusCode.InternalServerError, ErrorMessage = $"Error updating entity: {ex.Message}" };
            }
        }

        public async Task<ServiceResponse<T>> DeleteEntity<T>(string id) where T : BaseModel, new()
        {
            try
            {
                var entity = await _supabaseClient.From<T>().Select("*").Filter("id", Constants.Operator.Equals, id).Single();
                var response = await _supabaseClient.From<T>().Delete(entity);
                if (response.Content == null)
                {
                    return new ServiceResponse<T>
                    {
                        Status = HttpStatusCode.NotFound,
                        ErrorMessage = "Entity not found"
                    };
                }
                return new ServiceResponse<T> { Status = HttpStatusCode.OK, Data = response.Models.First() };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<T> { Status = HttpStatusCode.InternalServerError, ErrorMessage = $"Error deleting entity: {ex.Message}" };
            }
        }
    }
}