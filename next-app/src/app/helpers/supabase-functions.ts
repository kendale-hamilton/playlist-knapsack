import { supabase } from "@/lib/supabase";

export async function getCurrentUserId(): Promise<string | null> {
  try {
    // Get current authenticated user
    const {
      data: { user },
      error: authError,
    } = await supabase.auth.getUser();

    if (authError || !user) {
      return null;
    }

    return user.id;
  } catch (error) {
    console.error("Error in getCurrentUserId:", error);
    return null;
  }
}

export async function isSpotifyConnected(): Promise<boolean> {
  try {
    const userId = await getCurrentUserId();
    if (!userId) {
      return false;
    }

    // Check if user has Spotify connected
    const { data: userProfile } = await supabase
      .from("users")
      .select("spotify_user_id, spotify_access_token")
      .eq("id", userId)
      .single();

    return !!(
      userProfile?.spotify_user_id && userProfile?.spotify_access_token
    );
  } catch (error) {
    console.error("Error checking Spotify connection:", error);
    return false;
  }
}
