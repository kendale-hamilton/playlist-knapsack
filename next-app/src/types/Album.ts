import { Artist } from "./Artist"
import { ExternalUrls } from "./ExternalUrls"
import { Image } from "./Image"

export type Album = {
    available_markets: string[],
    type: string,
    album_type: string,
    href: string,
    id: string,
    images: Image[],
    name: string,
    release_date: string,
    release_date_precision: string,
    uri: string,
    artists: Artist[],
    external_urls: ExternalUrls
    total_tracks: number
}