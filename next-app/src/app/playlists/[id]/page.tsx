"use client";
import getCookies from "@/app/helpers/cookie-functions";
import { FullPlaylist } from "@/types/Playlist";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import PlaylistDetails from "./components/PlaylistDetails";
import BuilderConfiguration, { WeightingFunction } from "./components/BuilderConfiguration";
import { Track } from "@/types/Track";
import { Cookies } from "@/types/cookies";
import TrackList from "../components/TrackList";
import { fetchWithRetry } from "@/app/helpers/retry-fetch";
import { playlistDuration } from "@/app/helpers/time-functions";
import { useParams } from "next/navigation";

export type SubmissionProps = {
    desiredLength: number,
    max?: number,
    min?: number,
    weightingFunction: WeightingFunction
}

export default function Playlist() {
    const router = useRouter()
    const params = useParams()
    const { id } = params
    const [cookies, setCookies] = useState<Cookies | null>();
    const [submission, setSubmission] = useState<SubmissionProps | null>();

    useEffect(() => {
        const setCookieState = async () => {
            const cookieStore = getCookies();
            setCookies(cookieStore);
        }
        setCookieState()
    }, [])

    const [playlist, setPlaylist] = useState<FullPlaylist>()
    useEffect(() => {
        const fetchPlaylist = async () => {
            const cookies = getCookies();
            const res = await fetch(`${process.env.NEXT_PUBLIC_BACKEND_URL}/api/spotify/playlists/${id}`, {
                headers: {
                    Authorization: `Bearer ${cookies.accessToken}`
                }
            });
            return res;
        }

        const runFetchPlaylists = async () => {
            const response = await fetchWithRetry(fetchPlaylist)
            const playlist = await response.json()
            setPlaylist(playlist)
        }
        
        runFetchPlaylists()
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [])

    useEffect(() => {
        if (submission) {
        const postPlaylist = async (tracks?: Track[]) => {
            const body = {
                tracks: tracks,
                desiredLengths: {
                   length: submission.desiredLength,
                   max: submission.max,
                   min: submission.min
                }
            }
            const res = await fetch(`${process.env.NEXT_PUBLIC_BACKEND_URL}/api/knapsack/users/${cookies?.userId}/playlists`, 
            {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(body)
            })
            const json = await res.json()
            const customId = json.id
            router.push(`/playlists/custom/${customId}?desired-length=${submission.desiredLength}`)
        }

        const weightedTracks: Track[] = playlist ? playlist.tracks.map((track, index) => {
            return {
                ...track,
                weight: submission.weightingFunction(index, playlist.tracks.length)
            }
        }) : []

        postPlaylist(weightedTracks)
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [submission])

    if (!playlist) {
        return <div>Loading...</div>
    }
    
    const onSubmit = (props: SubmissionProps) => {
        const { desiredLength, max, min, weightingFunction } = props
        setSubmission({
            desiredLength: desiredLength,
            max: max ?? 0,
            min: min ?? 0,
            weightingFunction: weightingFunction
        })
    }

    return (
        <div className="bg-neutral-900 text-white h-fit">
            <div className="hidden md:flex flex-row w-full justify-center">
                <PlaylistDetails onSwitch={() => router.push('/playlists')} width="w-1/4" playlist={playlist} />
                <TrackList 
                    title="Tracks"
                    tracks={playlist.tracks} 
                    setPlaylist={(tracks: Track[]) => setPlaylist({...playlist, tracks: tracks})} 
                    width="w-1/2"
                />
                <BuilderConfiguration width="w-1/4" length={playlistDuration(playlist.tracks)} onSubmit={onSubmit}/>
            </div>
            <div className="flex flex-col md:hidden w-full">
                <PlaylistDetails onSwitch={() => router.push('/playlists')} width="w-full" playlist={playlist}  />
                <BuilderConfiguration width="w-full" length={playlistDuration(playlist.tracks)} onSubmit={onSubmit}/>
                <TrackList 
                    title="Tracks"
                    tracks={playlist.tracks} 
                    setPlaylist={(tracks: Track[]) => setPlaylist({...playlist, tracks: tracks})} 
                    width="w-full"
                />
            </div>
        </div>
    ) 
}