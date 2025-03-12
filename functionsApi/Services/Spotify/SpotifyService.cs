using System.Text;
using System.Text.Json;
using Models.Knapsack;
using Models.Spotify;
using Services.HttpService;

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

        public async Task<string> UploadPlaylist(string userId, Playlist playlist, string token)
        {
            SpotifyCreatePlaylistBody body = new SpotifyCreatePlaylistBody
            {
                Name = playlist.Details.Name,
                Description = playlist.Details.Description
            };
            string bodyJson = JsonSerializer.Serialize(body);
            HttpContent content = new StringContent(bodyJson, Encoding.UTF8, "application/json");
            var createResponse = await _httpService.MakePostRequest($"https://api.spotify.com/v1/users/{userId}/playlists", token, content);
            string createContent = await createResponse.Content.ReadAsStringAsync();
            SpotifyPlaylistsItem newPlaylist = JsonSerializer.Deserialize<SpotifyPlaylistsItem>(createContent);
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

            return url;
        }
    }
}