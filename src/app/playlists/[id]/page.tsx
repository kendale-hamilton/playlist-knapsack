"use client";
import getCookies from "@/app/helpers/get-cookies";
import { FullPlaylist } from "@/types/Playlist";
import { Button, Modal, ModalBody, ModalContent, ModalFooter, ModalHeader } from "@nextui-org/react";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import PlaylistDetails from "./PlaylistDetails";
import BuilderConfiguration from "./BuilderConfiguration";
import { Track } from "@/types/Track";
import { Cookies } from "@/types/cookies";
import TrackList from "../components/TrackList";


export default function Playlist({params}: any) {
    const router = useRouter()

    const [warningOn, setWarningOn] = useState(false)
    const [cookies, setCookies] = useState<Cookies | null>();
    const [submission, setSubmission] = useState<{
        desiredLength: number,
        weightingFunction: Function
    } | null>();

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
            const response = await fetch(`${process.env.NEXT_PUBLIC_BACKEND_URL}/spotify/playlists/${id}`, {
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
        if (!submission) {
            return
        }
        const postPlaylist = async (tracks?: Track[]) => {
            const body = {
                tracks: tracks,
                length: submission.desiredLength,
                userId: cookies?.userId
            }
            const res = await fetch(`${process.env.NEXT_PUBLIC_BACKEND_URL}/knapsack/playlists`, 
            {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(body)
            })
            const json = await res.json()
            const customId = json.id
            router.push(`/playlists/custom/${customId}`)
        }

        const weightedTracks: Track[] = playlist ? playlist.tracks.map((track, index) => {
            return {
                ...track,
                weight: submission.weightingFunction(index, playlist.tracks.length)
            }
        }) : []

        postPlaylist(weightedTracks)

    }, [submission])

    if (!playlist) {
        return <div>Loading...</div>
    }

    const onClose = () => { setWarningOn(false) }

    const onSubmit = (desiredLength: number, weightingFunction: Function) => {
        setSubmission({
            desiredLength: desiredLength,
            weightingFunction: weightingFunction
        })
    }

    return (
        <>
            <div className="flex h-full flex-row text-white">
                <PlaylistDetails playlist={playlist} setWarningOn={setWarningOn} />
                <TrackList tracks={playlist.tracks} setPlaylist={(tracks: Track[]) => setPlaylist({...playlist, tracks: tracks})} />
                <BuilderConfiguration onSubmit={onSubmit}/>
            </div>
            <Modal isOpen={warningOn} onClose={onClose}>
                <ModalContent>
                    {(onClose) => (
                        <div className="text-white">
                            <ModalHeader>Are you sure you want to leave this page?</ModalHeader>
                            <ModalBody>
                                <p>Any changes will not be saved.</p>
                            </ModalBody>
                            <ModalFooter>
                                <Button color="danger" onPress={onClose}>Cancel</Button>
                                <Button color="primary" onPress={() => router.push('/playlists')}>Leave</Button>
                            </ModalFooter>
                        </div>
                    )}
                </ModalContent>
            </Modal>
        </>
    ) 
}