"use client";
import getCookies from "@/app/helpers/cookie-functions";
import { FullPlaylist } from "@/types/Playlist";
import { Button, Modal, ModalBody, ModalContent, ModalFooter, ModalHeader, Tab, Tabs } from "@nextui-org/react";
import { redirect, useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import PlaylistDetails from "./PlaylistDetails";
import BuilderConfiguration from "./BuilderConfiguration";
import { Track } from "@/types/Track";
import { Cookies } from "@/types/cookies";
import TrackList from "../components/TrackList";

export type SubmissionProps = {
    desiredLength: number,
    max?: number,
    min?: number,
    weightingFunction: Function
}

export default function Playlist({params}: any) {
    const router = useRouter()
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
            const { id } = await params;
            const cookies = getCookies();
            const response = await fetch(`/api/spotify/playlists/${id}`, {
                headers: {
                    Authorization: `Bearer ${cookies.accessToken}`
                }
            });
            const fullPlaylist = await response.json()
            setPlaylist(fullPlaylist)
        }
        fetchPlaylist()
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
            const res = await fetch(`/api/knapsack/users/${cookies?.userId}/playlists`, 
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
        <div className="min-h-screen bg-neutral-900">
            <div className="flex flex-row text-white h-fit">
                <div className="hidden md:flex flex-row w-full">
                    <PlaylistDetails onSwitch={() => router.push('/playlists')} width="1/4" playlist={playlist} />
                    <TrackList 
                        title="Tracks"
                        tracks={playlist.tracks} 
                        setPlaylist={(tracks: Track[]) => setPlaylist({...playlist, tracks: tracks})} 
                        width="1/2"
                    />
                    <BuilderConfiguration width="1/4" onSubmit={onSubmit}/>
                </div>
                <div className="flex flex-col md:hidden w-full">
                    <PlaylistDetails onSwitch={() => router.push('/playlists')} width="full" playlist={playlist}  />
                    <BuilderConfiguration width="full" onSubmit={onSubmit}/>
                    <TrackList 
                        title="Tracks"
                        tracks={playlist.tracks} 
                        setPlaylist={(tracks: Track[]) => setPlaylist({...playlist, tracks: tracks})} 
                        width="full"
                    />
                </div>
            </div>
        </div>
    ) 
}