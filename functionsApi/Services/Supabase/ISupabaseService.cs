using Models.ServiceResponse;

namespace Services.SupabaseService
{
    public interface ISupabaseService
    {
        Task<ServiceResponse<string>> GetSpotifyUserId(string supabaseUserId);
        Task<ServiceResponse<string>> GetSpotifyAccessToken(string supabaseUserId);
        Task<ServiceResponse<string>> GetSpotifyRefreshToken(string supabaseUserId);
        Task<ServiceResponse<bool>> DisconnectSpotify(string supabaseUserId);
    }
} 