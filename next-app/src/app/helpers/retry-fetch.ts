import getCookies from "./cookie-functions"

export async function fetchWithRetry(fetchFunc: () => Promise<Response>): Promise<Response> {
    console.log("Fetching with retry")
    const cookies = getCookies()
    let response = await fetchFunc()
    if (response.status == 401) {
        const accessToken = await fetch(`${process.env.NEXT_PUBLIC_BACKEND_URL}/api/spotify/refresh/${cookies.refreshToken}`)
        console.log("Access token response: ", accessToken)
        document.cookie = `accessToken=${accessToken}`
        response = await fetchFunc()
    }
    return response
}