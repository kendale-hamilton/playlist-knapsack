"use client";
import { CustomPlaylist } from "@/types/Playlist";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { Card, CardBody, Image } from "@heroui/react";
import { useAuth } from "@/contexts/AuthContext";
import SpotifyConnectButton from "@/app/components/SpotifyConnectButton";

export default function CustomPlaylists() {
  const router = useRouter();
  const [playlists, setPlaylists] = useState<CustomPlaylist[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const { userId, spotifyConnected, loading: authLoading } = useAuth();

  useEffect(() => {
    const fetchCustomPlaylists = async () => {
      if (!userId || !spotifyConnected) return;

      setLoading(true);
      setError("");

      try {
        const response = await fetch(
          `${process.env.NEXT_PUBLIC_BACKEND_URL}/api/knapsack/users/${userId}/playlists`
        );

        if (!response.ok) {
          throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        setPlaylists(data);
      } catch (error) {
        console.error("Error fetching custom playlists:", error);
        setError("Failed to load custom playlists");
      } finally {
        setLoading(false);
      }
    };

    fetchCustomPlaylists();
  }, [userId, spotifyConnected]);

  if (authLoading) {
    return (
      <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full items-center justify-center">
        <div className="text-xl">Loading...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full items-center justify-center">
        <div className="text-xl text-red-400">{error}</div>
        <button
          onClick={() => router.push("/dashboard")}
          className="px-4 py-2 bg-blue-600 rounded-lg hover:bg-blue-700"
        >
          Go to Dashboard
        </button>
      </div>
    );
  }

  if (!spotifyConnected) {
    return (
      <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full items-center justify-center">
        <div className="text-xl text-center mb-4">
          Connect to Spotify to view your custom playlists
        </div>
        <div className="flex flex-col gap-4 items-center">
          <SpotifyConnectButton size="lg" />
          <button
            onClick={() => router.push("/dashboard")}
            className="px-4 py-2 bg-gray-600 rounded-lg hover:bg-gray-700"
          >
            Back to Dashboard
          </button>
        </div>
      </div>
    );
  }

  if (loading) {
    return (
      <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full items-center justify-center">
        <div className="text-xl">Loading playlists...</div>
      </div>
    );
  }

  if (!playlists.length) {
    return (
      <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full items-center justify-center">
        <div className="text-xl mb-4">No custom playlists found</div>
        <div className="text-center mb-6">
          <p className="text-gray-300 mb-4">
            Create your first custom playlist by selecting tracks from your
            Spotify playlists
          </p>
          <button
            onClick={() => router.push("/builder/playlists")}
            className="px-6 py-3 bg-purple-600 hover:bg-purple-700 rounded-lg font-semibold transition-colors"
          >
            Create Your First Playlist
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="m-4 text-center bg-neutral-900 overflow-x-hidden">
      <p className="text-purple-300 my-4 font-bold">Your Custom Playlists</p>
      <div className="flex justify-center">
        <div className="grid grid-cols-1 gap-8 md:grid-cols-3">
          {playlists.map((playlist) => (
            <Card
              isPressable
              onPress={() => router.push(`/playlists/${playlist.id}`)}
              className="bg-gray-500 w-56"
              key={playlist.id}
            >
              <CardBody className="flex flex-col gap-4 items-center">
                <p>{playlist.name}</p>
                <Image
                  alt="playlist image"
                  height={120}
                  radius="sm"
                  src={playlist.image_url || "/next.svg"}
                />
              </CardBody>
            </Card>
          ))}
        </div>
      </div>
    </div>
  );
}
