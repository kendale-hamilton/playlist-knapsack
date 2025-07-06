"use client";
import { Playlist } from "@/types/Playlist";
import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import {
  Card,
  CardBody,
  CardFooter,
  Divider,
  Image,
  Link,
  Button,
} from "@heroui/react";
import {
  getCurrentUserId,
  isSpotifyConnected,
} from "../helpers/supabase-functions";
import { supabase } from "@/lib/supabase";
import SpotifyConnectButton from "../components/SpotifyConnectButton";

export default function Builder() {
  const router = useRouter();
  const [playlists, setPlaylists] = useState<Playlist[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [spotifyConnected, setSpotifyConnected] = useState(false);

  useEffect(() => {
    const fetchPlaylists = async () => {
      // Check if user is authenticated
      const {
        data: { user },
      } = await supabase.auth.getUser();
      if (!user) {
        router.push("/auth/login");
        throw new Error("User not authenticated");
      }

      // Check if Spotify is connected
      const connected = await isSpotifyConnected();
      setSpotifyConnected(connected);

      if (!connected) {
        setLoading(false);
        return;
      }

      // Get current user ID
      const userId = await getCurrentUserId();
      console.log("User ID:", userId);

      if (!userId) {
        setError("User not authenticated");
        setLoading(false);
        throw new Error("User not authenticated");
      }

      console.log("Making API call with user ID:", userId);
      const res = await fetch(
        `${process.env.NEXT_PUBLIC_BACKEND_URL}/api/spotify/users/${userId}/playlists`
      );
      return res;
    };

    const runFetchPlaylists = async () => {
      try {
        const response = await fetchPlaylists();
        if (response) {
          const playlists = await response.json();
          setPlaylists(playlists);
        }
      } catch (error) {
        console.error("Error fetching playlists:", error);
        setError("Failed to load playlists");
      } finally {
        setLoading(false);
      }
    };

    runFetchPlaylists();
  }, [router]);

  if (loading) {
    return (
      <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full items-center justify-center">
        <div className="text-xl">Loading playlists...</div>
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
          Connect to Spotify to view your playlists
        </div>
        <div className="flex flex-col gap-4 items-center">
          <SpotifyConnectButton size="lg" />
          <Button
            color="secondary"
            onPress={() => router.push("/dashboard")}
            size="md"
          >
            Back to Dashboard
          </Button>
        </div>
      </div>
    );
  }

  if (!playlists.length) {
    return (
      <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full items-center justify-center">
        <div className="text-xl">No playlists found</div>
      </div>
    );
  }

  return (
    <div className="m-4 text-center bg-neutral-900 overflow-x-hidden">
      <p className="text-purple-300 my-4 font-bold">
        Select one of your playlists to begin
      </p>
      <div className="flex justify-center">
        <div className="grid grid-cols-1 gap-8 md:grid-cols-3">
          {playlists.map((playlist) => (
            <Card
              isPressable
              onPress={() => router.push(`playlists/${playlist.id}`)}
              className="bg-gray-500 w-56"
              key={playlist.id}
            >
              <CardBody className="flex flex-col gap-4 items-center">
                <p>{playlist.name}</p>
                <Image
                  alt="playlist image"
                  height={120}
                  radius="sm"
                  src={playlist.images[0].url}
                />
              </CardBody>
              <Divider />
              <CardFooter className="flex flex-row gap-2">
                <Link
                  isExternal
                  href={playlist.spotify_url}
                  className="text-purple-300 gap-2 font-bold"
                >
                  <Image
                    alt="spotify logo"
                    height={40}
                    radius="sm"
                    src="./spotify-svgrepo-com.svg"
                    width={40}
                  />
                  <p>View on Spotify</p>
                </Link>
              </CardFooter>
            </Card>
          ))}
        </div>
      </div>
    </div>
  );
}
