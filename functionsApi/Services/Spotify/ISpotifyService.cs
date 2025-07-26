using Models.Knapsack;
using Models.ServiceResponse;

namespace Services.SpotifyService
{
    public interface ISpotifyService
    {
        Task<ServiceResponse<string>> GetSpotifyUserId(string supaUserId);
        Task<ServiceResponse<List<PlaylistDetails>>> GetUserPlaylists(string userId, string token);
        Task<ServiceResponse<PlaylistDetails>> GetPlaylistDetails(string playlistId, string token);
        Task<ServiceResponse<List<Track>>> GetPlaylistTracks(string playlistId, string token);
        Task<ServiceResponse<string>> UploadPlaylist(string supabaseUserId, string spotifyUserId, Playlist playlist, string token);
        Task<ServiceResponse<string>> GetPlaylistImage(string playlistId, string token);
        Task<ServiceResponse<bool>> DeleteSpotifyPlaylist(string playlistId, string token);
        Task<string> RefreshAccessToken(string refreshToken);
        Task<ServiceResponse<string>> GetValidAccessToken(string supabaseUserId);
    }
}