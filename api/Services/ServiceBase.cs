using System.Text.Json;
using Models;

namespace API.Services
{
    public class ServiceBase : IServiceBase
    {

        public async Task<string> FetchPlaylists(string userId, string token)
        {
           throw new NotImplementedException();

        }
        
        public async Task<PlaylistItemsResponse> FetchPlaylistTracks(string playlistId, string token)
        {
            HttpClient client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.spotify.com/v1/playlists/{playlistId}/tracks");
            request.Headers.Add("Authorization", $"Bearer {token}");

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PlaylistItemsResponse>(content);
        }
    }
}