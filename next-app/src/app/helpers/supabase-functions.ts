import { supabase } from "@/lib/supabase";
import { Cookies } from "@/types/cookies";

export async function getUserFromSupabase(
  userId: string
): Promise<Cookies | null> {
  try {
    const { data: user, error } = await supabase
      .from("users")
      .select("*")
      .eq("id", userId)
      .single();

    if (error || !user) {
      console.error("Error fetching user:", error);
      return null;
    }

    return {
      userId: user.id,
      userDisplayName: user.display_name,
      userEmail: user.email,
      userAvatar: user.avatar_url || "",
      accessToken: user.spotify_access_token || "",
      refreshToken: user.spotify_refresh_token || "",
    };
  } catch (error) {
    console.error("Error in getUserFromSupabase:", error);
    return null;
  }
}

export async function getCurrentUserData(): Promise<Cookies | null> {
  try {
    // Get current authenticated user
    const {
      data: { user },
      error: authError,
    } = await supabase.auth.getUser();

    if (authError || !user) {
      return null;
    }

    // Get user profile with Spotify data
    const { data: userProfile, error: profileError } = await supabase
      .from("users")
      .select("*")
      .eq("id", user.id)
      .single();

    if (profileError || !userProfile) {
      return null;
    }

    return {
      userId: userProfile.id,
      userDisplayName: userProfile.display_name,
      userEmail: userProfile.email,
      userAvatar: userProfile.avatar_url || "",
      accessToken: userProfile.spotify_access_token || "",
      refreshToken: userProfile.spotify_refresh_token || "",
    };
  } catch (error) {
    console.error("Error in getCurrentUserData:", error);
    return null;
  }
}

export async function refreshSpotifyToken(
  refreshToken: string
): Promise<string | null> {
  try {
    const response = await fetch("https://accounts.spotify.com/api/token", {
      method: "POST",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
        Authorization: `Basic ${btoa(
          `${process.env.NEXT_PUBLIC_SPOTIFY_CLIENT_ID}:${process.env.SPOTIFY_CLIENT_SECRET}`
        )}`,
      },
      body: new URLSearchParams({
        grant_type: "refresh_token",
        refresh_token: refreshToken,
      }),
    });

    const { access_token, refresh_token: new_refresh_token } =
      await response.json();

    if (access_token) {
      // Update the token in Supabase if we got a new refresh token
      if (new_refresh_token) {
        // You might want to update the refresh token in the database here
        // For now, we'll just return the new access token
      }
      return access_token;
    }

    return null;
  } catch (error) {
    console.error("Error refreshing token:", error);
    return null;
  }
}

export async function getUserBySpotifyId(
  spotifyUserId: string
): Promise<Cookies | null> {
  try {
    const { data: user, error } = await supabase
      .from("users")
      .select("*")
      .eq("spotify_user_id", spotifyUserId)
      .single();

    if (error || !user) {
      return null;
    }

    return {
      userId: user.id,
      userDisplayName: user.display_name,
      userEmail: user.email,
      userAvatar: user.avatar_url || "",
      accessToken: user.spotify_access_token || "",
      refreshToken: user.spotify_refresh_token || "",
    };
  } catch (error) {
    console.error("Error in getUserBySpotifyId:", error);
    return null;
  }
}
