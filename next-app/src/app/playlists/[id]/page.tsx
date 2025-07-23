"use client";
import { Track } from "@/types/Track";
import { useEffect, useState } from "react";
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
import { LinkIcon } from "@heroicons/react/24/outline";
import { useSearchParams, useRouter, useParams } from "next/navigation";

import { useAuth } from "@/contexts/AuthContext";
import SpotifyConnectButton from "@/app/components/SpotifyConnectButton";
import TrackList from "@/app/components/TrackList";

export default function CustomPlaylist() {
  const params = useParams();
  const { id } = params as { id: string };
  const searchParams = useSearchParams();
  const desiredLength = searchParams.get("desired-length");

  const router = useRouter();

  const [customPlaylist, setCustomPlaylist] = useState<FullPlaylist>();
  const [spotifyPlaylist, setSpotifyPlaylist] = useState<FullPlaylist>();
  const [url, setUrl] = useState<string | null>();
  const [open, setOpen] = useState(false);
  const [playlistLoading, setPlaylistLoading] = useState(false);
  const [playlistError, setPlaylistError] = useState("");
  const [detailModalOpen, setDetailModalOpen] = useState(false);
  const { userId, spotifyConnected, loading, error } = useAuth();
  const [deleteLoading, setDeleteLoading] = useState(false);
  const [deleteError, setDeleteError] = useState("");

  useEffect(() => {
    console.log("useEffect called");
    console.log({ customPlaylist, userId, spotifyConnected });
    if (!customPlaylist && userId && spotifyConnected) {
      const fetchCustomPlaylist = async () => {
        setPlaylistLoading(true);
        try {
          const response = await fetch(
            `${process.env.NEXT_PUBLIC_BACKEND_URL}/api/knapsack/users/${userId}/playlists/${id}`
          );
          const customPlaylist = await response.json();
          console.log(customPlaylist);
          setCustomPlaylist(customPlaylist);
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
  }, [id, userId, spotifyConnected]);

  useEffect(() => {
    if (spotifyPlaylist && userId && spotifyConnected) {
      const postSpotifyPlaylist = async () => {
        const body = {
          playlist: spotifyPlaylist,
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
          const res = await response.json();
          setUrl(res.data);
          setOpen(true);
        } catch (error) {
          console.error("Error creating Spotify playlist:", error);
          setPlaylistError("Failed to create Spotify playlist");
        }
      };

      runPostSpotifyPlaylist();
    }
  }, [spotifyPlaylist, userId, spotifyConnected]);

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

  const handleDelete = async () => {
    if (
      !window.confirm(
        "Are you sure you want to delete this playlist? This action cannot be undone."
      )
    )
      return;
    setDeleteLoading(true);
    setDeleteError("");
    try {
      const res = await fetch(
        `${process.env.NEXT_PUBLIC_BACKEND_URL}/api/knapsack/users/${userId}/playlists/${id}`,
        { method: "DELETE" }
      );
      if (!res.ok) {
        const err = await res.text();
        setDeleteError(err || "Failed to delete playlist");
      } else {
        router.push("/playlists");
      }
    } catch (e) {
      setDeleteError("Failed to delete playlist");
    } finally {
      setDeleteLoading(false);
    }
  };

  return (
    <div className="flex flex-col text-white bg-neutral-900">
      <div className="flex flex-row items-center justify-between p-4 border-b border-gray-700">
        <div className="flex flex-row items-center space-x-4">
          <h1 className="text-xl font-bold">Playlist Details</h1>
        </div>
        <div className="flex flex-row gap-2 items-center">
          {!customPlaylist?.details.spotify_url && (
            <Button
              color="primary"
              onPress={() => setDetailModalOpen(true)}
              className="flex items-center gap-2"
            >
              <LinkIcon className="w-5 h-5" />
              Save to Spotify
            </Button>
          )}
          {customPlaylist?.details.spotify_url && (
            <Button
              color="primary"
              onPress={() => router.push(customPlaylist?.details.spotify_url)}
            >
              View in Spotify
            </Button>
          )}
          <Button
            color="danger"
            onPress={handleDelete}
            isLoading={deleteLoading}
            className="ml-2"
          >
            Delete Playlist
          </Button>
        </div>
      </div>
      {deleteError && (
        <div className="bg-red-500 text-white p-2 text-center">
          {deleteError}
        </div>
      )}

      <PlaylistDetailSelector
        isOpen={detailModalOpen}
        onClose={() => setDetailModalOpen(false)}
        id={id}
        tracks={customPlaylist?.tracks ?? []}
        desiredLength={Number(desiredLength)}
        setPlaylist={setSpotifyPlaylist}
      />

      <TrackList tracks={customPlaylist?.tracks ?? []} width="w-full" />

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
