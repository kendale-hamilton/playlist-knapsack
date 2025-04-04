"use client"
import { Playlist } from "@/types/Playlist"
import { useEffect, useState } from "react"
import getCookies from "../helpers/cookie-functions"
import { useRouter } from "next/navigation"
import { Card, CardBody, CardFooter, Divider, Image, Link } from "@heroui/react"
import { fetchWithRetry } from "../helpers/retry-fetch"

export default function Builder() {
    const router = useRouter()
    const [playlists, setPlaylists] = useState<Playlist[]>([])

    useEffect(() => {
        const fetchPlaylists = async () => {
            const cookies = getCookies();
            const res = await fetch(`${process.env.NEXT_PUBLIC_BACKEND_URL}/api/spotify/users/${cookies.userId}/playlists`, {
                headers: {
                    Authorization: `Bearer ${cookies.accessToken}`
                }
            })
            return res;
        }

        const runFetchPlaylists = async () => {
            const response = await fetchWithRetry(fetchPlaylists)
            const playlists = await response.json()
            setPlaylists(playlists)
        }


       
        runFetchPlaylists()
    }, [])

    if (!playlists.length) {
        return <p>Loading...</p>
    }

    return (
        <div className="m-4 text-center bg-neutral-900 overflow-x-hidden">
            <p className="text-purple-300 my-4 font-bold">Select one of your playlists to begin</p>
            <div className="flex justify-center">
                <div className="grid grid-cols-1 gap-8 md:grid-cols-3">
                    {playlists.map(playlist => (
                        <Card isPressable onPress={() => router.push(`playlists/${playlist.id}`)} className="bg-gray-500 w-56" key={playlist.id}>
                            <CardBody className="flex flex-col gap-4 items-center">
                                <p>{playlist.name}</p>
                                <Image
                                    alt="playlist image"
                                    height={120}
                                    radius="sm"
                                    src={playlist.images[0].url}
                                />
                            </CardBody>
                            <Divider />
                            <CardFooter className="flex flex-row gap-2">
                                <Link isExternal href={playlist.spotify_url} className="text-purple-300 gap-2 font-bold">
                                    <Image
                                        alt="spotify logo"
                                        height={40}
                                        radius="sm"
                                        src="./spotify-svgrepo-com.svg"
                                        width={40}
                                    />
                                    <p>View on Spotify</p>
                                </Link>
                            </CardFooter>
                        </Card>
                    ))}
                </div>
            </div>
        </div>

    )
}