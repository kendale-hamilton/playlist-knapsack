using Models.Knapsack;

namespace Services.SpotifyService
{
    public interface ISpotifyService
    {
        Task<List<PlaylistDetails>> GetUserPlaylists(string userId, string token);
        Task<PlaylistDetails> GetPlaylistDetails(string playlistId, string token);
        Task<List<Track>> GetPlaylistTracks(string playlistId, string token);
        Task<string> UploadPlaylist(string userId, Playlist playlist, string token);
    }
}