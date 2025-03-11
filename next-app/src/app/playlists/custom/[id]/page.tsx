"use client";
import getCookies from "@/app/helpers/cookie-functions";
import { Track } from "@/types/Track";
import { useEffect, useState } from "react";
import TrackList from "../../components/TrackList";
import PlaylistDetailSelector from "./components/PlaylistDetailSelector";
import { FullPlaylist } from "@/types/Playlist";


export default function CustomPlaylist({params}: any){
    const [id, setId] = useState<string>("")
    const [tracks, setTracks] = useState<Track[]>()
    const [playlist, setPlaylist] = useState<FullPlaylist>()
    useEffect(() => {
        if(!tracks){
            console.log("Fetching tracks")
            const fetchCustomPlaylist = async () => {
                const { id } = await params;
                setId(id)
                console.log("Fetching playlist: ", id)
                const cookies = getCookies();
                const response = await fetch(`/api/knapsack/users/${cookies.userId}/playlists/${id}`)
                const customPlaylist = await response.json()
                console.log("Response: ", customPlaylist)
                setTracks(customPlaylist)
            }
        
        fetchCustomPlaylist()
        }
    }, [])

    useEffect(() => {
        if (playlist)
        {
            const postSpotifyPlaylist = async () => {
                const cookies = getCookies();
                const response = await fetch(`/api/spotify/users/${cookies.userId}/playlists`, {
                    method: "POST",
                    headers: {
                        Authorization: `Bearer ${cookies.accessToken}`,
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(playlist)
                })
                const newPlaylist = await response.json()
                console.log("New Playlist: ", newPlaylist)
            }
            postSpotifyPlaylist()
        }
    }, [playlist])

    if (!tracks) {
        return <div>Loading...</div>
    }

    return (
        <div className="flex flex-row text-white">
            <PlaylistDetailSelector id={id} tracks={tracks} setPlaylist={setPlaylist}/>
            <TrackList tracks={tracks} classes="w-1/2" />
        </div>
    )

}