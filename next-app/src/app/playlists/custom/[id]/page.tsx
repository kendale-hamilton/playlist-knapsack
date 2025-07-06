"use client";
import { Track } from "@/types/Track";
import { useEffect, useState } from "react";
import TrackList from "../../components/TrackList";
import PlaylistDetailSelector from "./components/PlaylistDetailSelector";
import { FullPlaylist } from "@/types/Playlist";
import {
  Button,
  Link,
  Modal,
  ModalBody,
  ModalContent,
  ModalFooter,
  ModalHeader,
} from "@heroui/react";
import { useSearchParams, useRouter, useParams } from "next/navigation";
import { fetchWithRetry } from "@/app/helpers/retry-fetch";
import { getCurrentUserData } from "@/app/helpers/supabase-functions";
import { supabase } from "@/lib/supabase";

export default function CustomPlaylist() {
  const params = useParams();
  const { id } = params as { id: string };
  const searchParams = useSearchParams();
  const desiredLength = searchParams.get("desired-length");

  const router = useRouter();

  const [tracks, setTracks] = useState<Track[]>();
  const [playlist, setPlaylist] = useState<FullPlaylist>();
  const [url, setUrl] = useState<string | null>();
  const [open, setOpen] = useState(false);
  const [userData, setUserData] = useState<any>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const checkAuth = async () => {
      // Check if user is authenticated
      const {
        data: { user },
      } = await supabase.auth.getUser();
      if (!user) {
        router.push("/auth/login");
        return;
      }

      // Get user data with Spotify tokens
      const data = await getCurrentUserData();
      if (!data || !data.accessToken) {
        setError("Please connect your Spotify account first");
        setLoading(false);
        return;
      }

      setUserData(data);
      setLoading(false);
    };
    checkAuth();
  }, [router]);

  useEffect(() => {
    if (!tracks && userData) {
      const fetchCustomPlaylist = async () => {
        try {
          const response = await fetch(
            `${process.env.NEXT_PUBLIC_BACKEND_URL}/api/knapsack/users/${userData.userId}/playlists/${id}`
          );
          const customPlaylist = await response.json();
          setTracks(customPlaylist);
        } catch (error) {
          console.error("Error fetching custom playlist:", error);
          setError("Failed to load custom playlist");
        }
      };
      fetchCustomPlaylist();
    }
  }, [id, userData]);

  useEffect(() => {
    if (playlist && userData) {
      const postSpotifyPlaylist = async () => {
        const body = {
          playlist: playlist,
          // image: btoa(image || "")
        };
        const res = await fetch(
          `${process.env.NEXT_PUBLIC_BACKEND_URL}/api/spotify/users/${userData.userId}/playlists`,
          {
            method: "POST",
            headers: {
              Authorization: `Bearer ${userData.accessToken}`,
              "Content-Type": "application/json",
            },
            body: JSON.stringify(body),
          }
        );
        return res;
      };

      const runPostSpotifyPlaylist = async () => {
        try {
          const response = await fetchWithRetry(postSpotifyPlaylist);
          const url = await response.text();
          setUrl(url);
          setOpen(true);
        } catch (error) {
          console.error("Error creating Spotify playlist:", error);
          setError("Failed to create Spotify playlist");
        }
      };

      runPostSpotifyPlaylist();
    }
  }, [playlist, userData]);

  if (loading) {
    return (
      <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full min-h-screen items-center justify-center">
        <div className="text-xl">Loading custom playlist...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full min-h-screen items-center justify-center">
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

  if (!tracks) {
    return (
      <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full min-h-screen items-center justify-center">
        <div className="text-xl">Loading...</div>
      </div>
    );
  }

  return (
    <div className="flex flex-row text-white bg-neutral-900">
      <div className="hidden md:flex flex-row w-full justify-center">
        <PlaylistDetailSelector
          width="w-1/2"
          id={id}
          tracks={tracks}
          desiredLength={Number(desiredLength)}
          setPlaylist={setPlaylist}
        />
        <TrackList title="Selected Tracks" tracks={tracks} width="w-1/2" />
      </div>
      <div className="flex flex-col md:hidden w-full">
        <PlaylistDetailSelector
          width="w-full"
          id={id}
          tracks={tracks}
          desiredLength={Number(desiredLength)}
          setPlaylist={setPlaylist}
        />
        <TrackList title="Selected Tracks" tracks={tracks} width="w-full" />
      </div>
      <Modal isOpen={open} onClose={() => setOpen(false)}>
        <ModalContent>
          <div className="text-white">
            <ModalHeader>
              Your playlist has been uploaded to spotify!
            </ModalHeader>
            <ModalBody>
              <Link href={url ?? "/"} isExternal>
                Click here to view your playlist on spotify
              </Link>
            </ModalBody>
            <ModalFooter>
              <Button
                color="primary"
                onPress={() => {
                  router.push("/");
                }}
              >
                Home
              </Button>
            </ModalFooter>
          </div>
        </ModalContent>
      </Modal>
    </div>
  );
}
