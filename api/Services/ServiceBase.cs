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

        public async Task<SimpPlaylist> FetchPlaylist(string playlistId, string token)
        {
            var response = await MakeGetRequest($"https://api.spotify.com/v1/playlists/{playlistId}", token);
            string content = await response.Content.ReadAsStringAsync();
            PlaylistsItem? playlist = JsonSerializer.Deserialize<PlaylistsItem>(content);
            if (playlist == null)
            {
                throw new Exception("Failed to fetch playlist");
            }

            SimpPlaylist simpPlaylist = new SimpPlaylist
            {
                Details = playlist.Simplify()
            };

            return simpPlaylist;
        }
        
        public async Task<List<SimpTrack>> FetchPlaylistTracks(string playlistId, string token)
        {
            var response = await MakeGetRequest($"https://api.spotify.com/v1/playlists/{playlistId}/tracks", token);
            string content = await response.Content.ReadAsStringAsync();
            PlaylistItems? playlistItems = JsonSerializer.Deserialize<PlaylistItems>(content);
            
            List<SimpTrack> tracks = [];
            foreach (PlaylistTrack fullTrack in playlistItems.Items)
            {
                Track track = fullTrack.Track;
                var simpTrack = track.Simplify();
                tracks.Add(simpTrack);
            }

            return tracks;
        }
    }
}