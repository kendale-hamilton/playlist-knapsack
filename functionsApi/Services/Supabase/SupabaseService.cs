using System.Net;
using Models.ServiceResponse;
using Supabase.Postgrest;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Services.SupabaseService
{
    public class SupabaseService : ISupabaseService
    {
        private readonly Supabase.Client _supabaseClient;

        public SupabaseService()
        {
            Console.WriteLine("=== Supabase Service Initialization ===");
            
            var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL") ?? "";
            var supabaseServiceKey = Environment.GetEnvironmentVariable("SUPABASE_SERVICE_ROLE_KEY") ?? "";
            
            Console.WriteLine($"Supabase URL: {supabaseUrl}");
            Console.WriteLine($"Service key length: {supabaseServiceKey?.Length ?? 0}");
            Console.WriteLine($"Service key starts with: {supabaseServiceKey?.Substring(0, Math.Min(10, supabaseServiceKey?.Length ?? 0))}...");
            
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
            
            try
            {
                var options = new Supabase.SupabaseOptions
                {
                    AutoConnectRealtime = false
                };
                
                Console.WriteLine("Creating Supabase client...");
                _supabaseClient = new Supabase.Client(supabaseUrl, supabaseServiceKey, options);
                Console.WriteLine("Supabase client created successfully");
                
                Console.WriteLine("=== Supabase Service Initialization Complete ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Failed to create Supabase client: {ex.Message}");
                Console.WriteLine($"Exception type: {ex.GetType().Name}");
                throw;
            }
        }

        public async Task<ServiceResponse<string>> GetSpotifyUserId(string supabaseUserId)
        {
            try
            {
                Console.WriteLine("=== GetSpotifyUserId Method ===");
                Console.WriteLine($"Getting Spotify user ID for Supabase user: {supabaseUserId}");
                Console.WriteLine($"Supabase client is null: {_supabaseClient == null}");
                
                if (_supabaseClient == null)
                {
                    Console.WriteLine("ERROR: Supabase client is null!");
                    return new ServiceResponse<string>
                    {
                        Status = HttpStatusCode.InternalServerError,
                        ErrorMessage = "Supabase client is not initialized"
                    };
                }
                
                Console.WriteLine("Building query...");
                // Try to use the client's built-in methods
                var response = await _supabaseClient.From<UserRecord>()
                    .Select("spotify_user_id")
                    .Filter("id", Constants.Operator.Equals, supabaseUserId)
                    .Get();
                
                Console.WriteLine($"Supabase response: {response.Content}");
                
                if (response.Models == null || response.Models.Count == 0)
                {
                    return new ServiceResponse<string>
                    {
                        Status = HttpStatusCode.NotFound,
                        ErrorMessage = "User not found in database"
                    };
                }

                var user = response.Models.First();
                if (string.IsNullOrEmpty(user.SpotifyUserId))
                {
                    return new ServiceResponse<string>
                    {
                        Status = HttpStatusCode.NotFound,
                        ErrorMessage = "User found but Spotify not connected"
                    };
                }

                return new ServiceResponse<string>
                {
                    Status = HttpStatusCode.OK,
                    Data = user.SpotifyUserId
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetSpotifyUserId: {ex.Message}");
                return new ServiceResponse<string>
                {
                    Status = HttpStatusCode.InternalServerError,
                    ErrorMessage = $"Error fetching Spotify user ID: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<string>> GetSpotifyAccessToken(string supabaseUserId)
        {
            try
            {
                Console.WriteLine($"Getting Spotify access token for Supabase user: {supabaseUserId}");
                
                var response = await _supabaseClient.From<UserRecord>()
                    .Select("spotify_access_token")
                    .Filter("id", Constants.Operator.Equals, supabaseUserId)
                    .Get();

                if (response.Models == null || response.Models.Count == 0)
                {
                    return new ServiceResponse<string>
                    {
                        Status = HttpStatusCode.NotFound,
                        ErrorMessage = "User not found in database"
                    };
                }

                var user = response.Models.First();
                if (string.IsNullOrEmpty(user.SpotifyAccessToken))
                {
                    return new ServiceResponse<string>
                    {
                        Status = HttpStatusCode.NotFound,
                        ErrorMessage = "User found but Spotify access token not available"
                    };
                }

                return new ServiceResponse<string>
                {
                    Status = HttpStatusCode.OK,
                    Data = user.SpotifyAccessToken
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetSpotifyAccessToken: {ex.Message}");
                return new ServiceResponse<string>
                {
                    Status = HttpStatusCode.InternalServerError,
                    ErrorMessage = $"Error fetching Spotify access token: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<string>> GetSpotifyRefreshToken(string supabaseUserId)
        {
            try
            {
                Console.WriteLine($"Getting Spotify refresh token for Supabase user: {supabaseUserId}");
                
                var response = await _supabaseClient.From<UserRecord>()
                    .Select("spotify_refresh_token")
                    .Filter("id", Constants.Operator.Equals, supabaseUserId)
                    .Get();

                if (response.Models == null || response.Models.Count == 0)
                {
                    return new ServiceResponse<string>
                    {
                        Status = HttpStatusCode.NotFound,
                        ErrorMessage = "User not found in database"
                    };
                }

                var user = response.Models.First();
                if (string.IsNullOrEmpty(user.SpotifyRefreshToken))
                {
                    return new ServiceResponse<string>
                    {
                        Status = HttpStatusCode.NotFound,
                        ErrorMessage = "User found but Spotify refresh token not available"
                    };
                }

                return new ServiceResponse<string>
                {
                    Status = HttpStatusCode.OK,
                    Data = user.SpotifyRefreshToken
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetSpotifyRefreshToken: {ex.Message}");
                return new ServiceResponse<string>
                {
                    Status = HttpStatusCode.InternalServerError,
                    ErrorMessage = $"Error fetching Spotify refresh token: {ex.Message}"
                };
            }
        }
    }

    [Table("users")]
    public class UserRecord : BaseModel
    {
        [PrimaryKey("id")]
        public string? Id { get; set; }
        [Column("spotify_user_id")]
        public string? SpotifyUserId { get; set; }
        [Column("spotify_access_token")]
        public string? SpotifyAccessToken { get; set; }
        [Column("spotify_refresh_token")]
        public string? SpotifyRefreshToken { get; set; }
    }
} 