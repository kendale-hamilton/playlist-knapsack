using Models.Knapsack;
using Models.ServiceResponse;
using Supabase.Postgrest.Models;

namespace Services.SupabaseService
{
    public interface ISupabaseService
    {
        Task<ServiceResponse<string>> GetSpotifyUserId(string supabaseUserId);
        Task<ServiceResponse<string>> GetSpotifyAccessToken(string supabaseUserId);
        Task<ServiceResponse<string>> GetSpotifyRefreshToken(string supabaseUserId);
        Task<ServiceResponse<bool>> DisconnectSpotify(string supabaseUserId);
        Task<ServiceResponse<string>> UploadCustomPlaylist(List<Track> tracks, string userId);
        Task<ServiceResponse<string>> UpdateCustomPlaylist(string userId, Playlist playlist);
        Task<ServiceResponse<List<T>>> GetEntities<T>(List<string>? ids = null, string? columnName = null) where T : BaseModel, new();
    }
} 