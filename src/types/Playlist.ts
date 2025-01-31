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
    duration_ms: number
}

export type FullPlaylist = {
    details: Playlist,
    tracks: Track[]
}
