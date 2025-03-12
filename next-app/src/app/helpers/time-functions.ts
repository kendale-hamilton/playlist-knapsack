import { Track } from "@/types/Track"

// export function toTimeString(ms: number): string {
//     const seconds = Math.floor(ms / 1000)
//     const minutes = Math.floor(seconds / 60)
//     const hours = Math.floor(minutes / 60)
//     const remainingSeconds = seconds % 60

//     const output = `${minutes}:${remainingSeconds < 10 ? '0' : ''}${remainingSeconds}`
//     if (hours > 0) {
//         return `${hours}:${minutes < 10 ? '0' : ''}${output}`
//     }
//     return output
// }

export function toTimeStringSeconds(seconds: number): string {
    const minutes = Math.floor(seconds / 60)
    const remainingSeconds = seconds % 60
    const hours = Math.floor(minutes / 60)

    const output = `${minutes}:${remainingSeconds < 10 ? '0' : ''}${remainingSeconds}`
    if (hours > 0) {
        return `${hours}:${minutes < 10 ? '0' : ''}${output}`
    }
    return output
}


// export function toMs(timeString: string): number {
//     const [hours, minutes, seconds] = timeString.split(':').map(Number)
//     return (hours * 60 * 60 + minutes * 60 + seconds) * 1000
// }

export function toSecs(timeString: string): number {
    if (timeString === "") {
        return 0
    }
    const [hours, minutes, seconds] = timeString.split(':').map(Number)
    return (hours * 60 * 60 + minutes * 60 + seconds)
}

export function playlistDuration(tracks: Track[]): number {
    return tracks.reduce((total, track) => total + track.seconds, 0)
}
