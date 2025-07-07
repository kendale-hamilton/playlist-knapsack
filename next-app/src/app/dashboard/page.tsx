"use client";
import { useState } from "react";
import { useRouter } from "next/navigation";
import { Card, CardBody, Button, Avatar } from "@heroui/react";
import { supabase } from "@/lib/supabase";
import { useAuth } from "@/hooks/useAuth";

export default function Dashboard() {
  const [disconnecting, setDisconnecting] = useState(false);
  const router = useRouter();
  const { user, spotifyConnected, loading, refetch } = useAuth();

  const handleSignOut = async () => {
    await supabase.auth.signOut();
    // Refetch auth state to update the UI immediately
    await refetch();
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

  const disconnectSpotify = async () => {
    if (!user) {
      return;
    }

    if (!user.id) {
      return;
    }

    setDisconnecting(true);
    try {
      // Call the backend API to disconnect Spotify
      const response = await fetch(
        `${process.env.NEXT_PUBLIC_BACKEND_URL}/api/spotify/users/${user.id}/disconnect`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
        }
      );

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(
          errorData.message || "Failed to disconnect from Spotify"
        );
      }

      // The hook will automatically update the Spotify connection status
      // when the component re-renders after the disconnect
    } catch (error) {
      console.error("Error disconnecting Spotify:", error);
    } finally {
      setDisconnecting(false);
    }
  };

  if (loading) {
    return (
      <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full items-center justify-center">
        <div className="text-xl font-medium">Loading...</div>
      </div>
    );
  }

  return (
    <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full">
      <div className="max-w-4xl mx-auto w-full">
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-3xl font-bold text-white">Dashboard</h1>
          <Button color="danger" onPress={handleSignOut}>
            Sign Out
          </Button>
        </div>

        <div className="grid gap-6 md:grid-cols-2">
          {/* User Profile Card */}
          <Card className="bg-gray-800 border-2 border-gray-600 shadow-lg">
            <CardBody className="space-y-4">
              <h2 className="text-xl font-semibold text-white">Profile</h2>
              <div className="flex items-center gap-4">
                <Avatar
                  name={user?.email?.charAt(0).toUpperCase()}
                  className="w-16 h-16"
                />
                <div>
                  <p className="font-medium text-white">{user?.email}</p>
                  <p className="text-gray-300 text-sm">User ID: {user?.id}</p>
                </div>
              </div>
            </CardBody>
          </Card>

          {/* Spotify Connection Card */}
          <Card className="bg-gray-800 border-2 border-gray-600 shadow-lg">
            <CardBody className="space-y-4">
              <h2 className="text-xl font-semibold text-white">
                Spotify Connection
              </h2>

              {spotifyConnected ? (
                <div className="space-y-3">
                  <div className="flex items-center gap-2 text-green-300">
                    <div className="w-3 h-3 bg-green-400 rounded-full shadow-sm"></div>
                    <span className="font-medium">Connected to Spotify</span>
                  </div>
                  <div className="flex gap-2">
                    <Button
                      color="primary"
                      onPress={() => router.push("/playlists")}
                      className="flex-1"
                    >
                      Go to Playlists
                    </Button>
                    <Button
                      color="danger"
                      onPress={disconnectSpotify}
                      isLoading={disconnecting}
                      className="flex-1"
                    >
                      Disconnect
                    </Button>
                  </div>
                </div>
              ) : (
                <div className="space-y-3">
                  <div className="flex items-center gap-2 text-gray-300">
                    <div className="w-3 h-3 bg-gray-500 rounded-full shadow-sm"></div>
                    <span className="font-medium">
                      Not connected to Spotify
                    </span>
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
