using SpotifyAPI.Web;

namespace API.Services
{
    public interface IPlaylistService
    {
        Task<Paging<FullPlaylist>> GetPlaylists(string userId);
        Task<Paging<PlaylistTrack<IPlayableItem>>> GetPlaylistTracks(string playlistId);
    }
}