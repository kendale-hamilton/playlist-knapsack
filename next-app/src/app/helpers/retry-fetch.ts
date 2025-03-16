import getCookies from "./cookie-functions"

export async function fetchWithRetry(fetchFunc: () => Promise<Response>): Promise<Response> {
    const cookies = getCookies()
    let response = await fetchFunc()
    if (response.status == 401) {
        await fetch(`${process.env.NEXT_PUBLIC_BACKEND_URL}/api/spotify/refresh/${cookies.refreshToken}`)
        response = await fetchFunc()
    }
    return response
}