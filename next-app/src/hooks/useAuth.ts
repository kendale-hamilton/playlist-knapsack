import { useEffect, useState, useCallback } from "react";
import { useRouter } from "next/navigation";
import { supabase } from "@/lib/supabase";
import {
  getCurrentUserId,
  isSpotifyConnected,
} from "../app/helpers/supabase-functions";

export function useAuth() {
  const router = useRouter();
  const [user, setUser] = useState<any>(null);
  const [userId, setUserId] = useState<string | null>(null);
  const [spotifyConnected, setSpotifyConnected] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const checkAuth = useCallback(async () => {
    try {
      setLoading(true);
      setError("");

      // Check if user is authenticated
      const {
        data: { user },
      } = await supabase.auth.getUser();

      if (!user) {
        setUser(null);
        setUserId(null);
        setSpotifyConnected(false);
        return;
      }

      setUser(user);

      // Check if Spotify is connected
      const connected = await isSpotifyConnected();
      setSpotifyConnected(connected);

      // Get current user ID
      const currentUserId = await getCurrentUserId();
      setUserId(currentUserId);

      if (!currentUserId) {
        setError("User not authenticated");
      }
    } catch (error) {
      console.error("Auth error:", error);
      setError("Authentication failed");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    checkAuth();
  }, [checkAuth]);

  return {
    user,
    userId,
    spotifyConnected,
    loading,
    error,
    isAuthenticated: !!user,
    refetch: checkAuth,
  };
}
