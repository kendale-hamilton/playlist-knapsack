
export default function toTimeString(ms: number): string {
    const seconds = Math.floor(ms / 1000)
    const minutes = Math.floor(seconds / 60)
    const hours = Math.floor(minutes / 60)
    const remainingSeconds = seconds % 60

    const output = `${minutes}:${remainingSeconds < 10 ? '0' : ''}${remainingSeconds}`
    if (hours > 0) {
        return `${hours}:${minutes < 10 ? '0' : ''}${output}`
    }
    return output
}