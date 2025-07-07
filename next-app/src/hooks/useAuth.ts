import { useEffect, useState } from "react";
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

  useEffect(() => {
    const checkAuth = async () => {
      try {
        // Check if user is authenticated
        const {
          data: { user },
        } = await supabase.auth.getUser();
        if (!user) {
          router.push("/auth/login");
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
    };

    checkAuth();
  }, [router]);

  return {
    user,
    userId,
    spotifyConnected,
    loading,
    error,
    isAuthenticated: !!user,
  };
}
