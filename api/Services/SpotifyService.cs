using System.Text.Json;
using Models.Knapsack;
using Models.Spotify;


namespace Services.SpotifyService
{
    public class SpotifyService : ISpotifyService
    {
        private readonly IHttpService _httpService;
        public SpotifyService(IHttpService httpService)
        {
            _httpService = httpService;
        }
        public async Task<List<PlaylistDetails>> GetUserPlaylists(string userId, string token)
        {
            var response = await _httpService.MakeGetRequest($"https://api.spotify.com/v1/users/{userId}/playlists", token);
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
            var response = await _httpService.MakeGetRequest($"https://api.spotify.com/v1/playlists/{playlistId}", token);
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
            var response = await _httpService.MakeGetRequest($"https://api.spotify.com/v1/playlists/{playlistId}/tracks", token);
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