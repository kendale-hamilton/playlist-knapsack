"use client";
import getCookies from "@/app/helpers/get-cookies";
import { FullPlaylist } from "@/types/Playlist";
import { useEffect, useState } from "react";


export default function Playlist({params}: any) {
    const [playlist, setPlaylist] = useState<FullPlaylist>()
    useEffect(() => {
        const fetchPlaylist = async () => {
            const { id } = await params;
            const cookies = getCookies();
            const response = await fetch(`${process.env.NEXT_PUBLIC_BACKEND_URL}/users/${cookies.userId}/playlists/${id}/tracks`, {
                headers: {
                    Authorization: `Bearer ${cookies.accessToken}`
                }
            })
            const fullPlaylist = await response.json()
            console.log('Setting playlist: ', fullPlaylist)
            setPlaylist(fullPlaylist)
        }
        fetchPlaylist()
        console.log('Playlist: ', playlist)
    }, [])

    if (!playlist) {
        return <div>Loading...</div>
    }

    return (
        <div className="text-white">
            <h1>Playlist: {playlist.details.name}</h1>
        </div>
    ) 
}