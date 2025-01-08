using SpotifyAPI.Web;

namespace API.Services
{
    public interface IServiceBase
    {
        Task<Paging<FullPlaylist>> FetchPlaylists(string userId);
        Task<Paging<PlaylistTrack<IPlayableItem>>> FetchPlaylistTracks(string playlistId);
    }
}