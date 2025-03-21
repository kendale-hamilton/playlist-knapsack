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
}

export type FullPlaylist = {
    details: Playlist,
    tracks: Track[]
}
