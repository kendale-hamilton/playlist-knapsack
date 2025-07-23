"use client";
import {
  createContext,
  useContext,
  useEffect,
  useState,
  useCallback,
  ReactNode,
} from "react";
import { supabase } from "@/lib/supabase";
import { User } from "@supabase/supabase-js";
import {
  getCurrentUserId,
  isSpotifyConnected,
} from "../app/helpers/supabase-functions";

interface AuthContextType {
  user: User | null;
  userId: string | null;
  spotifyConnected: boolean;
  spotifyUser: {
    display_name: string | null;
    avatar_url: string | null;
    spotify_user_id: string | null;
  } | null;
  loading: boolean;
  error: string;
  isAuthenticated: boolean;
  refetch: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [userId, setUserId] = useState<string | null>(null);
  const [spotifyConnected, setSpotifyConnected] = useState(false);
  const [spotifyUser, setSpotifyUser] = useState<{
    display_name: string | null;
    avatar_url: string | null;
    spotify_user_id: string | null;
  } | null>(null);
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
        setSpotifyUser(null);
        return;
      }

      setUser(user);

      // Get current user ID
      const currentUserId = await getCurrentUserId();
      setUserId(currentUserId);

      if (!currentUserId) {
        setError("User not authenticated");
        return;
      }

      // Check if Spotify is connected and get user data
      const connected = await isSpotifyConnected();
      setSpotifyConnected(connected);

      if (connected) {
        // Fetch Spotify user data from database
        const { data: userProfile } = await supabase
          .from("users")
          .select("display_name, avatar_url, spotify_user_id")
          .eq("id", currentUserId)
          .single();

        if (userProfile) {
          setSpotifyUser({
            display_name: userProfile.display_name,
            avatar_url: userProfile.avatar_url,
            spotify_user_id: userProfile.spotify_user_id,
          });
        }
      } else {
        setSpotifyUser(null);
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

    // Listen for auth state changes
    const {
      data: { subscription },
    } = supabase.auth.onAuthStateChange((event, session) => {
      console.log("Auth state changed:", event, session?.user?.email);
      checkAuth();
    });

    return () => subscription.unsubscribe();
  }, [checkAuth]);

  const value = {
    user,
    userId,
    spotifyConnected,
    spotifyUser,
    loading,
    error,
    isAuthenticated: !!user,
    refetch: checkAuth,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within AuthProvider");
  }
  return context;
}
