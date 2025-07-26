using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Models.Knapsack;
using Models.ServiceResponse;
using Models.Spotify;
using Models.Supabase;
using Services.Base;
using Supabase.Postgrest;

namespace Services.SpotifyService
{
    public class SpotifyService : BaseService, ISpotifyService
    {
        public SpotifyService() : base() {}

        public async Task<ServiceResponse<string>> GetSpotifyUserId(string supaUserId)
        {
            try
            {
                if (_supabaseClient == null)
                {
                    return new ServiceResponse<string>
                    {
                        Status = HttpStatusCode.InternalServerError,
                        ErrorMessage = "Supabase client is not initialized"
                    };
                }
                
                var response = await _supabaseClient.From<UserRecord>()
                    .Select("spotify_user_id")
                    .Filter("id", Constants.Operator.Equals, supaUserId)
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

        public async Task<ServiceResponse<bool>> DisconnectSpotify(string supaUserId)
        {
            try
            {
                var userResponse = await GetEntities<UserRecord>([supaUserId], "id");
                if (userResponse.Status != HttpStatusCode.OK)
                {
                    return new ServiceResponse<bool>
                    {
                        Status = HttpStatusCode.NotFound,
                        ErrorMessage = "User not found in database"
                    };
                }

                var updatedRecord = new UserRecord
                {
                    Id = supaUserId,
                    SpotifyUserId = null,
                    SpotifyAccessToken = null,
                    SpotifyRefreshToken = null
                };
                var updateResponse = await UpdateEntity(updatedRecord);

                if (updateResponse.Status != HttpStatusCode.OK)
                {
                    return new ServiceResponse<bool>
                    {
                        Status = HttpStatusCode.InternalServerError,
                        ErrorMessage = "Failed to update user record"
                    };
                }

                return new ServiceResponse<bool>
                {
                    Status = HttpStatusCode.OK,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DisconnectSpotify: {ex.Message}");
                return new ServiceResponse<bool>
                {
                    Status = HttpStatusCode.InternalServerError,
                    ErrorMessage = $"Error disconnecting Spotify: {ex.Message}"
                };
            }
        }

         public async Task<ServiceResponse<string>> GetValidAccessToken(string supabaseUserId)
        {
            try
            {
                var userResponse = await GetEntities<UserRecord>([supabaseUserId], "id");
                if (userResponse.Status != HttpStatusCode.OK)
                {
                    return new ServiceResponse<string>
                    {
                        Status = HttpStatusCode.NotFound,
                        ErrorMessage = "User not found"
                    };
                }
                var userRecord = userResponse.Data.First();
                var accessToken = userRecord.SpotifyAccessToken;
                var refreshToken = userRecord.SpotifyRefreshToken;

                // Test the current token with a simple Spotify API call
                var testResponse = await MakeGetRequest("https://api.spotify.com/v1/me", accessToken, "Bearer");
                
                if (testResponse.StatusCode == HttpStatusCode.OK)
                {
                    // Token is still valid
                    return new ServiceResponse<string>
                    {
                        Status = HttpStatusCode.OK,
                        Data = accessToken
                    };
                }
                else if (testResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    string newAccessToken = await RefreshAccessToken(refreshToken);
                    if (string.IsNullOrEmpty(newAccessToken))
                    {
                        return new ServiceResponse<string>
                        {
                            Status = HttpStatusCode.BadRequest,
                            ErrorMessage = "Failed to refresh access token"
                        };
                    }
                    
                    userRecord.SpotifyAccessToken = newAccessToken;
                    var updateResponse = await UpdateEntity(userRecord);
                    if (updateResponse.Status != HttpStatusCode.OK)
                    {
                        return new ServiceResponse<string>
                        {
                            Status = HttpStatusCode.InternalServerError,
                            ErrorMessage = "Failed to update access token"
                        };
                    }
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
        
        public async Task<ServiceResponse<List<PlaylistDetails>>> GetUserPlaylists(string userId, string token)
        {
            var response = await MakeGetRequest($"https://api.spotify.com/v1/users/{userId}/playlists", token, "Bearer");
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
            var response = await MakeGetRequest($"https://api.spotify.com/v1/playlists/{playlistId}", token, "Bearer");
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
            var response = await MakeGetRequest($"https://api.spotify.com/v1/playlists/{playlistId}/tracks", token, "Bearer");
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

        public async Task<ServiceResponse<string>> UploadPlaylist(string supabaseUserId, string spotifyUserId, Playlist playlist, string token)
        {
            SpotifyCreatePlaylistBody body = new SpotifyCreatePlaylistBody
            {
                Name = playlist.Details.Name,
                Description = playlist.Details.Description
            };
            string bodyJson = JsonSerializer.Serialize(body);
            HttpContent content = new StringContent(bodyJson, Encoding.UTF8, "application/json");
            var createResponse = await MakePostRequest($"https://api.spotify.com/v1/users/{spotifyUserId}/playlists", token, content);
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
                var addResponse = await MakePostRequest($"https://api.spotify.com/v1/playlists/{id}/tracks", token, addContent);
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

            var updatedRecord = new CustomPlaylistRecord
            {
                Id = playlist.Details.Id,
                UserId = supabaseUserId,
                Name = playlist.Details.Name,
                SpotifyId = id,
                SpotifyUrl = url
            };

            var updatedResponse = await UpdateEntity(updatedRecord);
            if (updatedResponse.Status != HttpStatusCode.OK)
            {
                return new ServiceResponse<string>
                {
                    Status = HttpStatusCode.InternalServerError,
                    ErrorMessage = "Failed to update playlist in Supabase"
                };
            }

            return new ServiceResponse<string>
            {
                Status = HttpStatusCode.OK,
                Data = url
            };
        }

        public async Task<ServiceResponse<string>> GetPlaylistImage(string playlistId, string token)
        {
            Console.WriteLine("Getting playlist image");
            Console.WriteLine($"Playlist ID: {playlistId}");
            Console.WriteLine($"Token: {token}");
            var response = await _httpService.MakeGetRequest($"https://api.spotify.com/v1/playlists/{playlistId}/images", token, "Bearer");
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new ServiceResponse<string>
                {
                    Status = HttpStatusCode.Unauthorized,
                    ErrorMessage = "Unauthorized"
                };
            }
            string content = await response.Content.ReadAsStringAsync();
            var image = JsonSerializer.Deserialize<List<SpotifyImage>>(content);

            return new ServiceResponse<string>
            {
                Status = HttpStatusCode.OK,
                Data = image.FirstOrDefault()?.Url ?? ""
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
        
        public async Task<ServiceResponse<bool>> DeleteSpotifyPlaylist(string playlistId, string token)
        {
            var response = await _httpService.MakeDeleteRequest($"https://api.spotify.com/v1/playlists/{playlistId}/followers", token, "Bearer");
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return new ServiceResponse<bool> { Status = System.Net.HttpStatusCode.OK, Data = true };
            }
            return new ServiceResponse<bool>
            {
                Status = response.StatusCode,
                ErrorMessage = "Failed to delete playlist from Spotify"
            };
        }
    }
}