import { Track } from "./Track"

export type Playlist = {
    id: string
    name: string
    description: string
    images: 
    [
        {
            url: string
            width: number
            height: number
        }
    ]
    seconds: number
    spotify_url: string
}

export type FullPlaylist = {
    details: Playlist,
    tracks: Track[]
}
