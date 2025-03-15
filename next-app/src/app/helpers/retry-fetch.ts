import getCookies from "./cookie-functions"

export async function fetchWithRetry(fetchFunc: () => Promise<Response>): Promise<Response> {
    const cookies = getCookies()
    var response = await fetchFunc()
    if (response.status == 401) {
        await fetch(`/api/spotify/refresh/${cookies.refreshToken}`)
        var response = await fetchFunc()
    }
    return response
}