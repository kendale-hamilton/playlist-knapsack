"use client";
import { FullPlaylist } from "@/types/Playlist";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import PlaylistDetails from "./components/PlaylistDetails";
import BuilderConfiguration, {
  WeightingFunction,
} from "./components/BuilderConfiguration";
import { Track } from "@/types/Track";
import TrackList from "../components/TrackList";
import { fetchWithRetry } from "@/app/helpers/retry-fetch";
import { playlistDuration } from "@/app/helpers/time-functions";
import { useParams } from "next/navigation";
import { getCurrentUserData } from "@/app/helpers/supabase-functions";
import { supabase } from "@/lib/supabase";

export type SubmissionProps = {
  desiredLength: number;
  max?: number;
  min?: number;
  weightingFunction: WeightingFunction;
};

export default function Playlist() {
  const router = useRouter();
  const params = useParams();
  const { id } = params;
  const [userData, setUserData] = useState<any>(null);
  const [submission, setSubmission] = useState<SubmissionProps | null>();
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

  const [playlist, setPlaylist] = useState<FullPlaylist>();
  useEffect(() => {
    if (!userData) return;

    const fetchPlaylist = async () => {
      const res = await fetch(
        `${process.env.NEXT_PUBLIC_BACKEND_URL}/api/spotify/users/${userData.userId}/playlists/${id}`,
        {
          headers: {
            Authorization: `Bearer ${userData.accessToken}`,
          },
        }
      );
      return res;
    };

    const runFetchPlaylists = async () => {
      try {
        const response = await fetchWithRetry(fetchPlaylist);
        const playlist = await response.json();
        setPlaylist(playlist);
      } catch (error) {
        console.error("Error fetching playlist:", error);
        setError("Failed to load playlist");
      }
    };

    runFetchPlaylists();
  }, [id, userData]);

  useEffect(() => {
    if (submission && userData) {
      const postPlaylist = async (tracks?: Track[]) => {
        const body = {
          tracks: tracks,
          desiredLengths: {
            length: submission.desiredLength,
            max: submission.max,
            min: submission.min,
          },
        };
        const res = await fetch(
          `${process.env.NEXT_PUBLIC_BACKEND_URL}/api/knapsack/users/${userData.userId}/playlists`,
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify(body),
          }
        );
        const json = await res.json();
        const customId = json.id;
        router.push(
          `/playlists/custom/${customId}?desired-length=${submission.desiredLength}`
        );
      };

      const weightedTracks: Track[] = playlist
        ? playlist.tracks.map((track, index) => {
            return {
              ...track,
              weight: submission.weightingFunction(
                index,
                playlist.tracks.length
              ),
            };
          })
        : [];

      postPlaylist(weightedTracks);
    }
  }, [submission, userData, playlist, router]);

  if (loading) {
    return (
      <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full items-center justify-center">
        <div className="text-xl">Loading playlist...</div>
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

  if (!playlist) {
    return (
      <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full items-center justify-center">
        <div className="text-xl">Loading...</div>
      </div>
    );
  }

  const onSubmit = (props: SubmissionProps) => {
    const { desiredLength, max, min, weightingFunction } = props;
    setSubmission({
      desiredLength: desiredLength,
      max: max ?? 0,
      min: min ?? 0,
      weightingFunction: weightingFunction,
    });
  };

  return (
    <div className="bg-neutral-900 text-white h-fit">
      <div className="hidden md:flex flex-row w-full justify-center">
        <PlaylistDetails
          onSwitch={() => router.push("/playlists")}
          width="w-1/4"
          playlist={playlist}
        />
        <TrackList
          title="Tracks"
          tracks={playlist.tracks}
          setPlaylist={(tracks: Track[]) =>
            setPlaylist({ ...playlist, tracks: tracks })
          }
          width="w-1/2"
        />
        <BuilderConfiguration
          width="w-1/4"
          length={playlistDuration(playlist.tracks)}
          onSubmit={onSubmit}
        />
      </div>
      <div className="flex flex-col md:hidden w-full">
        <PlaylistDetails
          onSwitch={() => router.push("/playlists")}
          width="w-full"
          playlist={playlist}
        />
        <BuilderConfiguration
          width="w-full"
          length={playlistDuration(playlist.tracks)}
          onSubmit={onSubmit}
        />
        <TrackList
          title="Tracks"
          tracks={playlist.tracks}
          setPlaylist={(tracks: Track[]) =>
            setPlaylist({ ...playlist, tracks: tracks })
          }
          width="w-full"
        />
      </div>
    </div>
  );
}
