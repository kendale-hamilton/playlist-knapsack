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

import { useAuth } from "@/hooks/useAuth";
import SpotifyConnectButton from "../../../components/SpotifyConnectButton";

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
  const [playlistLoading, setPlaylistLoading] = useState(false);
  const [playlistError, setPlaylistError] = useState("");
  const { userId, spotifyConnected, loading, error } = useAuth();

  useEffect(() => {
    if (!tracks && userId && spotifyConnected) {
      const fetchCustomPlaylist = async () => {
        setPlaylistLoading(true);
        try {
          const response = await fetch(
            `${process.env.NEXT_PUBLIC_BACKEND_URL}/api/knapsack/users/${userId}/playlists/${id}`
          );
          const customPlaylist = await response.json();
          setTracks(customPlaylist);
        } catch (error) {
          console.error("Error fetching custom playlist:", error);
          setPlaylistError("Failed to load custom playlist");
        } finally {
          setPlaylistLoading(false);
        }
      };
      fetchCustomPlaylist();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [id, userId]);

  useEffect(() => {
    if (playlist && userId && spotifyConnected) {
      const postSpotifyPlaylist = async () => {
        const body = {
          playlist: playlist,
          // image: btoa(image || "")
        };
        const res = await fetch(
          `${process.env.NEXT_PUBLIC_BACKEND_URL}/api/spotify/users/${userId}/playlists`,
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify(body),
          }
        );
        return res;
      };

      const runPostSpotifyPlaylist = async () => {
        try {
          const response = await postSpotifyPlaylist();
          const url = await response.text();
          setUrl(url);
          setOpen(true);
        } catch (error) {
          console.error("Error creating Spotify playlist:", error);
          setPlaylistError("Failed to create Spotify playlist");
        }
      };

      runPostSpotifyPlaylist();
    }
  }, [playlist, userId, spotifyConnected]);

  if (loading) {
    return (
      <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full items-center justify-center">
        <div className="text-xl">Loading custom playlist...</div>
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
          Connect to Spotify to create and upload playlists
        </div>
        <div className="flex flex-col gap-4 items-center">
          <SpotifyConnectButton size="lg" />
          <Button
            color="secondary"
            onPress={() => router.push("/playlists")}
            size="md"
          >
            Back to Playlists
          </Button>
        </div>
      </div>
    );
  }

  if (playlistLoading) {
    return (
      <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full items-center justify-center">
        <div className="text-xl">Loading custom playlist...</div>
      </div>
    );
  }

  if (playlistError) {
    return (
      <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full items-center justify-center">
        <div className="text-xl text-red-400">{playlistError}</div>
        <button
          onClick={() => router.push("/playlists")}
          className="px-4 py-2 bg-blue-600 rounded-lg hover:bg-blue-700"
        >
          Back to Playlists
        </button>
      </div>
    );
  }

  if (!tracks) {
    return (
      <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full items-center justify-center">
        <div className="text-xl">No tracks found</div>
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
