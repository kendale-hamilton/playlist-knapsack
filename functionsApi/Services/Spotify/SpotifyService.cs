using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Models.Knapsack;
using Models.ServiceResponse;
using Models.Spotify;
using Services.HttpService;
using Services.SupabaseService;

namespace Services.SpotifyService
{
    public class SpotifyService : ISpotifyService
    {
        private readonly IHttpService _httpService;
        private readonly ISupabaseService _supabaseService;
        
        public SpotifyService(IHttpService httpService, ISupabaseService supabaseService)
        {
            _httpService = httpService;
            _supabaseService = supabaseService;
        }
        public async Task<ServiceResponse<List<PlaylistDetails>>> GetUserPlaylists(string userId, string token)
        {
            var response = await _httpService.MakeGetRequest($"https://api.spotify.com/v1/users/{userId}/playlists", token, "Bearer");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new ServiceResponse<List<PlaylistDetails>>
                {
                    Status = HttpStatusCode.Unauthorized,
                    ErrorMessage = "Unauthorized"
                };
            }
            string content = await response.Content.ReadAsStringAsync();
            SpotifyUserPlaylists? jsonResponse = JsonSerializer.Deserialize<SpotifyUserPlaylists>(content);

            if (jsonResponse == null || jsonResponse.Items == null)
            {
                return new ServiceResponse<List<PlaylistDetails>>
                {
                    Status = HttpStatusCode.NotFound,
                    ErrorMessage = "Failed to fetch users playlists"
                };
            }

            List<PlaylistDetails> playlists = [];
            foreach (SpotifyPlaylistsItem item in jsonResponse.Items)
            {   
                if (item != null)
                {
                    PlaylistDetails details = item.Simplify();
                    playlists.Add(details);
                }
            }
            return new ServiceResponse<List<PlaylistDetails>>
            {
                Status = HttpStatusCode.OK,
                Data = playlists
            };
        }

        public async Task<ServiceResponse<PlaylistDetails>> GetPlaylistDetails(string playlistId, string token)
        {
            var response = await _httpService.MakeGetRequest($"https://api.spotify.com/v1/playlists/{playlistId}", token, "Bearer");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new ServiceResponse<PlaylistDetails>
                {
                    Status = HttpStatusCode.Unauthorized,
                    ErrorMessage = "Unauthorized"
                };
            }
            string content = await response.Content.ReadAsStringAsync();
            SpotifyPlaylistsItem? playlist = JsonSerializer.Deserialize<SpotifyPlaylistsItem>(content);
            if (playlist == null)
            {
                return new ServiceResponse<PlaylistDetails>
                {
                    Status = HttpStatusCode.NotFound,
                    ErrorMessage = "Failed to fetch playlist details"
                };
            }

            PlaylistDetails simpPlaylist = playlist.Simplify();

            return new ServiceResponse<PlaylistDetails>
            {
                Status = HttpStatusCode.OK,
                Data = simpPlaylist
            };
        }
        
        public async Task<ServiceResponse<List<Track>>> GetPlaylistTracks(string playlistId, string token)
        {
            var response = await _httpService.MakeGetRequest($"https://api.spotify.com/v1/playlists/{playlistId}/tracks", token, "Bearer");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new ServiceResponse<List<Track>>
                {
                    Status = HttpStatusCode.Unauthorized,
                    ErrorMessage = "Unauthorized"
                };
            }
            string content = await response.Content.ReadAsStringAsync();
            SpotifyPlaylistItems? playlistItems = JsonSerializer.Deserialize<SpotifyPlaylistItems>(content);
            if (playlistItems == null || playlistItems.Items == null)
            {
                return new ServiceResponse<List<Track>>
                {
                    Status = HttpStatusCode.NotFound,
                    ErrorMessage = "Failed to fetch playlist tracks"
                };
            }
            
            List<Track> tracks = [];
            foreach (SpotifyPlaylistTrack fullTrack in playlistItems.Items)
            {
                SpotifyTrack track = fullTrack.Track;
                Track simpleTrack = track.Simplify();
                tracks.Add(simpleTrack);
            }

            return new ServiceResponse<List<Track>>
            {
                Status = HttpStatusCode.OK,
                Data = tracks
            };
        }

        public async Task<ServiceResponse<string>> UploadPlaylist(string userId, Playlist playlist, string token)
        {
            SpotifyCreatePlaylistBody body = new SpotifyCreatePlaylistBody
            {
                Name = playlist.Details.Name,
                Description = playlist.Details.Description
            };
            string bodyJson = JsonSerializer.Serialize(body);
            HttpContent content = new StringContent(bodyJson, Encoding.UTF8, "application/json");
            var createResponse = await _httpService.MakePostRequest($"https://api.spotify.com/v1/users/{userId}/playlists", token, content);
            if (createResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new ServiceResponse<string>
                {
                    Status = HttpStatusCode.Unauthorized,
                    ErrorMessage = "Unauthorized"
                };
            }
            string createContent = await createResponse.Content.ReadAsStringAsync();
            SpotifyPlaylistsItem newPlaylist = JsonSerializer.Deserialize<SpotifyPlaylistsItem>(createContent);
            if (newPlaylist == null)
            {
                return new ServiceResponse<string>
                {
                    Status = HttpStatusCode.NotFound,
                    ErrorMessage = "Failed to create playlist"
                };
            }

            string id = newPlaylist.Id;
            string url = newPlaylist.ExternalUrls.Spotify;

            List<string> allUris = [.. playlist.Tracks.Select(t => t.Uri)];

            for (int i = 0; i < allUris.Count; i += 100)
            {
                int n = 100;
                if (i + n >= allUris.Count)
                {
                    n = allUris.Count - i;
                }
                List<string> uris = allUris.GetRange(i, n);
                SpotifyAddTracksBody addBody = new SpotifyAddTracksBody
                {
                    Uris = uris
                };
                string addBodyJson = JsonSerializer.Serialize(addBody);
                HttpContent addContent = new StringContent(addBodyJson, Encoding.UTF8, "application/json");
                var addResponse = await _httpService.MakePostRequest($"https://api.spotify.com/v1/playlists/{id}/tracks", token, addContent);
                if (addResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return new ServiceResponse<string>
                    {
                        Status = HttpStatusCode.Unauthorized,
                        ErrorMessage = "Unauthorized"
                    };
                }
            }

            // TODO: Add Custom Image here
            // Console.WriteLine("Image: " + image);
            // if (image != null)
            // {
            //     string replaced = image.Replace("data:image/jpeg;base64,", "");
            //     byte[] imageBytes = Convert.FromBase64String(image);
            //     HttpContent imageContent = new ByteArrayContent(imageBytes);
            //     var imageResponse = await _httpService.MakePutRequest($"https://api.spotify.com/v1/playlists/{id}/images", token, imageContent, "image/jpeg");
            // }

            return new ServiceResponse<string>
            {
                Status = HttpStatusCode.OK,
                Data = url
            };
        }
        public async Task<string> RefreshAccessToken(string refreshToken)
        {
            var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");
            string auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            SpotifyRefreshTokenBody body = new SpotifyRefreshTokenBody
            {
                GrantType = "refresh_token",
                RefreshToken = refreshToken
            };
            string bodyJson = JsonSerializer.Serialize(body);
            var formContent = new Dictionary<string, string>
            {
                { "grant_type", body.GrantType },
                { "refresh_token", body.RefreshToken }
            };
            HttpContent content = new FormUrlEncodedContent(formContent);

            var response = await _httpService.MakePostRequest("https://accounts.spotify.com/api/token", auth, content, "Basic");
            string tokenContent = await response.Content.ReadAsStringAsync();
            var tokenJson = JsonObject.Parse(tokenContent);
            string accessToken = tokenJson["access_token"]?.ToString();
            return accessToken;
        }
        
        public async Task<ServiceResponse<string>> GetValidAccessToken(string supabaseUserId)
        {
            try
            {
                // First, try to get the current access token
                var accessTokenResponse = await _supabaseService.GetSpotifyAccessToken(supabaseUserId);
                if (accessTokenResponse.Status != HttpStatusCode.OK)
                {
                    return new ServiceResponse<string>
                    {
                        Status = HttpStatusCode.NotFound,
                        ErrorMessage = "User not found or Spotify access token not available"
                    };
                }

                // Test the current token with a simple Spotify API call
                var testResponse = await _httpService.MakeGetRequest("https://api.spotify.com/v1/me", accessTokenResponse.Data, "Bearer");
                
                if (testResponse.StatusCode == HttpStatusCode.OK)
                {
                    // Token is still valid
                    return new ServiceResponse<string>
                    {
                        Status = HttpStatusCode.OK,
                        Data = accessTokenResponse.Data
                    };
                }
                else if (testResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // Token is expired, refresh it
                    var refreshTokenResponse = await _supabaseService.GetSpotifyRefreshToken(supabaseUserId);
                    if (refreshTokenResponse.Status != HttpStatusCode.OK)
                    {
                        return new ServiceResponse<string>
                        {
                            Status = HttpStatusCode.NotFound,
                            ErrorMessage = "Refresh token not available"
                        };
                    }

                    string newAccessToken = await RefreshAccessToken(refreshTokenResponse.Data);
                    if (string.IsNullOrEmpty(newAccessToken))
                    {
                        return new ServiceResponse<string>
                        {
                            Status = HttpStatusCode.BadRequest,
                            ErrorMessage = "Failed to refresh access token"
                        };
                    }

                    // TODO: Update the access token in Supabase database
                    // For now, we'll just return the new token
                    return new ServiceResponse<string>
                    {
                        Status = HttpStatusCode.OK,
                        Data = newAccessToken
                    };
                }
                else
                {
                    return new ServiceResponse<string>
                    {
                        Status = testResponse.StatusCode,
                        ErrorMessage = "Unexpected error testing access token"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<string>
                {
                    Status = HttpStatusCode.InternalServerError,
                    ErrorMessage = $"Error getting valid access token: {ex.Message}"
                };
            }
        }
    }
}