using System.Text.Json;
using Models;

namespace API.Services
{
    public class ServiceBase : IServiceBase
    {
        private readonly HttpClient _client = new HttpClient();
        private async Task<HttpResponseMessage> MakeGetRequest(string url, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return response;
        }

        public async Task<List<SimpUserPlaylistItem>> GetUserPlaylists(string userId, string token)
        {
            var response = await MakeGetRequest($"https://api.spotify.com/v1/users/{userId}/playlists", token);
            string content = await response.Content.ReadAsStringAsync();
            UserPlaylists? jsonResponse = JsonSerializer.Deserialize<UserPlaylists>(content);

            if (jsonResponse == null || jsonResponse.Items == null)
            {
                throw new Exception("Failed to fetch user playlists");
            }

            List<SimpUserPlaylistItem> playlists = [];
            foreach (var item in jsonResponse.Items)
            {   
                if (item != null)
                {
                    var simpPlaylist = item.Simplify();
                    playlists.Add(simpPlaylist);
                }
            }
            return playlists;
        }
        
        public async Task<PlaylistItems> FetchPlaylistTracks(string playlistId, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.spotify.com/v1/playlists/{playlistId}/tracks");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PlaylistItems>(content);
        }
    }
}