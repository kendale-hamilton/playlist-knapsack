"use client";
import getCookies from "@/app/helpers/get-cookies";
import { FullPlaylist } from "@/types/Playlist";
import { Button, Modal, ModalBody, ModalContent, ModalFooter, ModalHeader } from "@nextui-org/react";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import Tracks from "./Tracks";
import PlaylistDetails from "./PlaylistDetails";
import BuilderConfiguration from "./BuilderConfiguration";
import { SimpleTrack } from "@/types/Track";


export default function Playlist({params}: any) {
    const router = useRouter()

    const [warningOn, setWarningOn] = useState(false)

    const [playlist, setPlaylist] = useState<FullPlaylist>()
    useEffect(() => {
        const fetchPlaylist = async () => {
            const { id } = await params;
            const cookies = getCookies();
            const response = await fetch(`${process.env.NEXT_PUBLIC_BACKEND_URL}/spotify/playlists/${id}`, {
                headers: {
                    Authorization: `Bearer ${cookies.accessToken}`
                }
            })
            const fullPlaylist = await response.json()
            setPlaylist(fullPlaylist)
        }
        fetchPlaylist()
    }, [])

    if (!playlist) {
        return <div>Loading...</div>
    }

    const onClose = () => { setWarningOn(false) }

    const onSubmit = (desiredLength: string, weightingFunction: Function) => {
        const postPlaylist = async (simplePlaylist: SimpleTrack[]) => {
            const res = await fetch(`${process.env.NEXT_PUBLIC_BACKEND_URL}/knapsack/playlists?length=${desiredLength}`, 
            {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(simplePlaylist)
            })

            return res.json()
        }
        
        const simpleTracks: SimpleTrack[] = playlist.tracks.map((track, index) => {
            return {
                name: track.name,
                duration_ms: track.duration_ms,
                weight: weightingFunction(index, playlist.tracks.length)
            }
        })

        postPlaylist(simpleTracks)

        console.log("Weighted Playlist: ", simpleTracks)
    }

    return (
        <>
            <div className="flex h-full flex-row text-white">
                <PlaylistDetails playlist={playlist} setWarningOn={setWarningOn} />
                <Tracks playlist={playlist} setPlaylist={setPlaylist} />
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