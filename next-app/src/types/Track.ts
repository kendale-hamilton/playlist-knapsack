import { Album } from "./Album"
import { Artist } from "./Artist"

export type Track = {
    album: Album
    artists: Artist[]
    seconds: number
    name: string
    popularity: number
    weight: number
    uri: string
}
