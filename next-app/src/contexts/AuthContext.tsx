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
import {
  getCurrentUserId,
  isSpotifyConnected,
} from "../app/helpers/supabase-functions";

interface AuthContextType {
  user: any;
  userId: string | null;
  spotifyConnected: boolean;
  loading: boolean;
  error: string;
  isAuthenticated: boolean;
  refetch: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
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
