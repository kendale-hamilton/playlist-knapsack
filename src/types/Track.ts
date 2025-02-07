export type Track = {
    album: any,
    artists: any[],
    duration_ms: number,
    name: string,
    popularity: number
}

export type SimpleTrack = {
    name: string,
    duration_ms: number,
    weight: number
}