"use client";
import { useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { Card, CardBody, Button, Avatar } from "@heroui/react";
import { supabase } from "@/lib/supabase";
import { User } from "@supabase/supabase-js";

export default function Dashboard() {
  const [user, setUser] = useState<User | null>(null);
  const [spotifyConnected, setSpotifyConnected] = useState(false);
  const [loading, setLoading] = useState(true);
  const router = useRouter();

  const checkUser = async () => {
    try {
      const {
        data: { user },
      } = await supabase.auth.getUser();

      if (!user) {
        router.push("/auth/login");
        return;
      }

      setUser(user);

      // Check if user has Spotify connected
      const { data: userProfile } = await supabase
        .from("users")
        .select("spotify_user_id, spotify_access_token")
        .eq("id", user.id)
        .single();

      if (userProfile?.spotify_user_id && userProfile?.spotify_access_token) {
        setSpotifyConnected(true);
      }
    } catch (error) {
      console.error("Error checking user:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    checkUser();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const handleSignOut = async () => {
    await supabase.auth.signOut();
    router.push("/");
  };

  const connectSpotify = () => {
    if (!user) return;
    const state = JSON.stringify({
      returnUrl: window.location.href,
      userId: user.id,
    });
    const params = new URLSearchParams({
      response_type: "code",
      client_id: process.env.NEXT_PUBLIC_SPOTIFY_CLIENT_ID!,
      scope:
        "playlist-read-private playlist-read-collaborative playlist-modify-public playlist-modify-private user-read-email ugc-image-upload",
      redirect_uri: process.env.NEXT_PUBLIC_SPOTIFY_REDIRECT_URI!,
      state: state,
      show_dialog: "true",
    });
    window.location.href = `https://accounts.spotify.com/authorize?${params.toString()}`;
  };

  if (loading) {
    return (
      <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full items-center justify-center">
        <div className="text-xl">Loading...</div>
      </div>
    );
  }

  return (
    <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full">
      <div className="max-w-4xl mx-auto w-full">
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-3xl font-bold">Dashboard</h1>
          <Button color="danger" onPress={handleSignOut}>
            Sign Out
          </Button>
        </div>

        <div className="grid gap-6 md:grid-cols-2">
          {/* User Profile Card */}
          <Card className="bg-gray-800 border border-gray-600">
            <CardBody className="space-y-4">
              <h2 className="text-xl font-semibold">Profile</h2>
              <div className="flex items-center gap-4">
                <Avatar
                  name={user?.email?.charAt(0).toUpperCase()}
                  className="w-16 h-16"
                />
                <div>
                  <p className="font-medium">{user?.email}</p>
                  <p className="text-gray-400 text-sm">User ID: {user?.id}</p>
                </div>
              </div>
            </CardBody>
          </Card>

          {/* Spotify Connection Card */}
          <Card className="bg-gray-800 border border-gray-600">
            <CardBody className="space-y-4">
              <h2 className="text-xl font-semibold">Spotify Connection</h2>

              {spotifyConnected ? (
                <div className="space-y-3">
                  <div className="flex items-center gap-2 text-green-400">
                    <div className="w-3 h-3 bg-green-400 rounded-full"></div>
                    <span>Connected to Spotify</span>
                  </div>
                  <Button
                    color="primary"
                    onPress={() => router.push("/playlists")}
                    className="w-full"
                  >
                    Go to Playlists
                  </Button>
                </div>
              ) : (
                <div className="space-y-3">
                  <div className="flex items-center gap-2 text-gray-400">
                    <div className="w-3 h-3 bg-gray-400 rounded-full"></div>
                    <span>Not connected to Spotify</span>
                  </div>
                  <Button
                    color="primary"
                    onPress={connectSpotify}
                    className="w-full"
                  >
                    Connect Spotify
                  </Button>
                </div>
              )}
            </CardBody>
          </Card>
        </div>
      </div>
    </div>
  );
}
