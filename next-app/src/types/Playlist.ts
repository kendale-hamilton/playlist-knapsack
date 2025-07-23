import { Track } from "./Track";

export type Playlist = {
  id: string;
  name: string;
  description: string;
  images: [
    {
      url: string;
      width: number;
      height: number;
    }
  ];
  seconds: number;
  spotify_url: string;
};

export type FullPlaylist = {
  details: Playlist;
  tracks: Track[];
};

export type CustomPlaylist = {
  id: string;
  name: string;
  image_url?: string;
  spotify_url: string;
};
