using System.Net;
using Models.ServiceResponse;
using Supabase.Postgrest;
using Models.Supabase;
using Models.Knapsack;
using Supabase.Postgrest.Models;
using System.Text.Json;

namespace Services.SupabaseService
{
    public class SupabaseService
    {
        private readonly Supabase.Client _supabaseClient;

        public SupabaseService()
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
            
            try
            {
                var options = new Supabase.SupabaseOptions
                {
                    AutoConnectRealtime = false
                };
                
                _supabaseClient = new Supabase.Client(supabaseUrl, supabaseServiceKey, options);
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
                if (_supabaseClient == null)
                {
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

        public async Task<ServiceResponse<bool>> DisconnectSpotify(string supabaseUserId)
        {
            try
            {
                Console.WriteLine($"Disconnecting Spotify for Supabase user: {supabaseUserId}");
                
                // Validate input
                if (string.IsNullOrEmpty(supabaseUserId))
                {
                    Console.WriteLine("ERROR: supabaseUserId is null or empty");
                    return new ServiceResponse<bool>
                    {
                        Status = HttpStatusCode.BadRequest,
                        ErrorMessage = "User ID is required"
                    };
                }
                
                // First check if user exists
                var checkResponse = await _supabaseClient.From<UserRecord>()
                    .Select("id")
                    .Filter("id", Constants.Operator.Equals, supabaseUserId)
                    .Get();

                if (checkResponse.Models == null || checkResponse.Models.Count == 0)
                {
                    return new ServiceResponse<bool>
                    {
                        Status = HttpStatusCode.NotFound,
                        ErrorMessage = "User not found in database"
                    };
                }

                var updateResponse = await _supabaseClient.From<UserRecord>()
                    .Filter("id", Constants.Operator.Equals, supabaseUserId)
                    .Update(new UserRecord
                    {
                        Id = supabaseUserId,
                        SpotifyUserId = null,
                        SpotifyAccessToken = null,
                        SpotifyRefreshToken = null
                    });

                Console.WriteLine($"Update response: {updateResponse.Content}");

                Console.WriteLine($"Successfully disconnected Spotify tokens for user: {supabaseUserId}");

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

        public async Task<ServiceResponse<string>> UploadCustomPlaylist(List<Track> tracks, string userId)
        {
            try
            {
                Console.WriteLine("Uploading custom playlist to Supabase");
                Console.WriteLine($"User ID: {userId}");
                Console.WriteLine($"Uploading {tracks.Count} tracks");
                var trackIds = (await Task.WhenAll(tracks.Select(async track => 
                {
                    var existingTrack = await _supabaseClient
                        .From<TrackRecord>()
                        .Where(x => x.SpotifyId == track.SpotifyId)
                        .Single();

                    if (existingTrack != null)
                    {
                        Console.WriteLine($"Track '{track.Name}' already exists, skipping");
                        return existingTrack.Id;
                    }

                    var trackRes = await _supabaseClient.From<TrackRecord>().Insert(new TrackRecord
                    {
                        SpotifyId = track.SpotifyId,
                        Seconds = track.Seconds,
                        Name = track.Name,
                        SpotifyUrl = track.SpotifyUrl,
                        Uri = track.Uri,
                        ArtistsId = null,
                        AlbumId = null
                    });

                    return trackRes.Model.Id;
                }))).ToList();

                Console.WriteLine($"Track IDs: {string.Join(", ", trackIds)}");

                var playlistRes = await _supabaseClient.From<CustomPlaylistRecord>().Insert(new CustomPlaylistRecord
                {
                    UserId = userId,
                    Name = "Custom Playlist"
                });

                var playlistId = playlistRes.Model.Id;

                await Task.WhenAll(trackIds.Select(async trackId => 
                {
                    var trackRes = await _supabaseClient.From<PlaylistTrackRecord>().Insert(new PlaylistTrackRecord
                    {
                        PlaylistId = playlistId,
                        TrackId = trackId,
                    });

                    return trackRes.Model.Id;
                }));

                return new ServiceResponse<string>
                {
                    Status = HttpStatusCode.OK,
                    Data = playlistId
                };
            } catch (Exception ex)
            {
                Console.WriteLine($"Error uploading custom playlist: {ex.Message}");
                return new ServiceResponse<string>
                {
                    Status = HttpStatusCode.InternalServerError,
                    ErrorMessage = $"Error uploading custom playlist: {ex.Message}"
                };
            }
        }  

        public async Task<ServiceResponse<string>> UpdateCustomPlaylist(string userId, CustomPlaylistDetails details)
        {
            try
            {
                Console.WriteLine("Updating custom playlist in Supabase");
                Console.WriteLine($"User ID: {userId}");
                Console.WriteLine($"Playlist: {JsonSerializer.Serialize(details)}");

                var updateResponse = await _supabaseClient.From<CustomPlaylistRecord>()
                    .Filter("id", Constants.Operator.Equals, details.Id)
                    .Update(new CustomPlaylistRecord
                    {
                        Id = details.Id,
                        UserId = userId,
                        Name = details.Name,
                        SpotifyId = details.SpotifyId,
                        SpotifyUrl = details.SpotifyUrl,
                        ImageUrl = details.ImageUrl
                    });
                
                return new ServiceResponse<string>
                {
                    Status = HttpStatusCode.OK,
                    Data = updateResponse.Model.Id
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating custom playlist: {ex.Message}");
                return new ServiceResponse<string>
                {
                    Status = HttpStatusCode.InternalServerError,
                    ErrorMessage = $"Error updating custom playlist: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteCustomPlaylist(string playlistId)
        {
            try
            {
                await _supabaseClient.From<CustomPlaylistRecord>()
                    .Filter("id", Constants.Operator.Equals, playlistId)
                    .Delete();

                return new ServiceResponse<bool>
                {
                    Status = HttpStatusCode.OK,
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Status = HttpStatusCode.InternalServerError,
                    ErrorMessage = $"Error deleting custom playlist: {ex.Message}"
                };
            }
        }

        public async Task<ServiceResponse<List<T>>> GetEntities<T>(List<string>? ids = null, string? columnName = null) where T : BaseModel, new()
        {
            try
            {
                var query = _supabaseClient.From<T>().Select("*");

                if (ids != null && ids.Count > 0)
                {
                    query = query.Filter(columnName ?? "id", Constants.Operator.In, ids);
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
    }
} 