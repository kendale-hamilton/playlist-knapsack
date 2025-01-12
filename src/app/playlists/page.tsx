"use client"

import { Playlist } from "@/types/Playlist"
import { useSession } from "next-auth/react"
import { useEffect, useState } from "react"

export default function Builder() {
    const { data: session} = useSession()
    const [playlists, setPlaylists] = useState<Playlist[]>([])
    console.log({session})

    useEffect(() => {
        const fetchPlaylists = async () => {
            const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/playlists`, {
                headers: {
                    Authorization: `Bearer ${session?.user.token}`
                }
            })
            const playlists = await response.json()
            console.log('Setting playlists: ', playlists)
            setPlaylists(playlists)
        }

        fetchPlaylists()
    }, [])

    return (
        <div>
            <p className="text-purple-300">Select one of your playlists to begin</p>
            {playlists.map(playlist => (
                <div key={playlist.id}>
                    <p>{playlist.name}</p>
                </div>
            ))}
        </div>

    )
}