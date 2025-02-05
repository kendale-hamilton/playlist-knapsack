using System.Text.Json;
using Models.Spotify;
using Models.Knapsack;

namespace Services
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

        public async Task<List<PlaylistDetails>> GetUserPlaylists(string userId, string token)
        {
            var response = await MakeGetRequest($"https://api.spotify.com/v1/users/{userId}/playlists", token);
            string content = await response.Content.ReadAsStringAsync();
            SpotifyUserPlaylists? jsonResponse = JsonSerializer.Deserialize<SpotifyUserPlaylists>(content);

            if (jsonResponse == null || jsonResponse.Items == null)
            {
                throw new Exception("Failed to fetch user playlists");
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
            return playlists;
        }

        public async Task<PlaylistDetails> GetPlaylistDetails(string playlistId, string token)
        {
            var response = await MakeGetRequest($"https://api.spotify.com/v1/playlists/{playlistId}", token);
            string content = await response.Content.ReadAsStringAsync();
            SpotifyPlaylistsItem? playlist = JsonSerializer.Deserialize<SpotifyPlaylistsItem>(content);
            if (playlist == null)
            {
                throw new Exception("Failed to fetch playlist");
            }

            PlaylistDetails simpPlaylist = playlist.Simplify();

            return simpPlaylist;
        }
        
        public async Task<List<Track>> GetPlaylistTracks(string playlistId, string token)
        {
            var response = await MakeGetRequest($"https://api.spotify.com/v1/playlists/{playlistId}/tracks", token);
            string content = await response.Content.ReadAsStringAsync();
            SpotifyPlaylistItems? playlistItems = JsonSerializer.Deserialize<SpotifyPlaylistItems>(content);
            
            List<Track> tracks = [];
            foreach (SpotifyPlaylistTrack fullTrack in playlistItems.Items)
            {
                SpotifyTrack track = fullTrack.Track;
                Track simpleTrack = track.Simplify();
                tracks.Add(simpleTrack);
            }

            return tracks;
        }
    }
}