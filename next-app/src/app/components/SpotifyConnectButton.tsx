"use client";
import { Button } from "@heroui/react";
import { supabase } from "@/lib/supabase";
import { getCurrentUserId } from "../helpers/supabase-functions";

interface SpotifyConnectButtonProps {
  className?: string;
  variant?: "primary" | "secondary";
  size?: "sm" | "md" | "lg";
}

export default function SpotifyConnectButton({
  className = "",
  variant = "primary",
  size = "md",
}: SpotifyConnectButtonProps) {
  const connectSpotify = async () => {
    const userId = await getCurrentUserId();
    if (!userId) return;

    const state = JSON.stringify({
      returnUrl: window.location.href,
      userId: userId,
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

  return (
    <Button
      color={variant}
      size={size}
      onPress={connectSpotify}
      className={className}
    >
      Connect to Spotify
    </Button>
  );
}
