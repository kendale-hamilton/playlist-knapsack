"use client";
import getCookies from "@/app/helpers/get-cookies";
import { Track } from "@/types/Track";
import { useEffect, useState } from "react";
import TrackList from "../../components/TrackList";


export default function CustomPlaylist({params}: any){
    const [tracks, setTracks] = useState<Track[]>()
    useEffect(() => {
        console.log("UseEffect triggered")
        const fetchCustomPlaylist = async () => {
            const { id } = await params;
            console.log("Fetching playlist: ", id)
            const cookies = getCookies();
            const response = await fetch(`${process.env.NEXT_PUBLIC_BACKEND_URL}/knapsack/playlists/${cookies.userId}/${id}`)
            const customPlaylist = await response.json()
            console.log("Response: ", customPlaylist)
            setTracks(customPlaylist)
        }

        fetchCustomPlaylist()
    }, [])

    if (!tracks) {
        return <div>Loading...</div>
    }
    console.log(tracks)

    return (
        <TrackList tracks={tracks} />
    )

}