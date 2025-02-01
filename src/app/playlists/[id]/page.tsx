"use client";
import getCookies from "@/app/helpers/get-cookies";
import toTimeString from "@/app/helpers/ms-convert";
import { FullPlaylist } from "@/types/Playlist";
import { Track } from "@/types/Track";
import { ArrowsRightLeftIcon, ChevronDownIcon, ChevronUpIcon } from "@heroicons/react/16/solid";
import { Button, Card, Modal, ModalBody, ModalContent, ModalFooter, ModalHeader } from "@nextui-org/react";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";


export default function Playlist({params}: any) {
    const router = useRouter()

    const [warningOn, setWarningOn] = useState(false)

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
            console.log("fullPlaylist: ", fullPlaylist)
            setPlaylist(fullPlaylist)
        }
        fetchPlaylist()
        console.log('Playlist: ', playlist)
    }, [])

    if (!playlist) {
        return <div>Loading...</div>
    }

    const swapTracks = (index_1: number, index_2: number) => {
        const newTracks: Track[] = [...playlist.tracks];
        [newTracks[index_1], newTracks[index_2]] = [newTracks[index_2], newTracks[index_1]];
        setPlaylist({ ...playlist, tracks: newTracks });
    }

    const playlistDisplay = (
        <div className="w-1/4 flex flex-col m-2 p-4 space-y-4 items-center">
            <div className="font-bold underline">Selected Playlist</div>
            <img className="w-32 h-32" src={playlist.details.images[0].url} />
            <p>{playlist.details.name}</p>
            <p>Length: {toTimeString(playlist.details.duration_ms)}</p>
            <Button color="primary" onPress={() => setWarningOn(true)}>
                <p>Switch Playlist</p>
                <ArrowsRightLeftIcon className="w-6 h-6"/>
            </Button>
        </div>
    )

    const trackSelector = (
        <div className="w-1/2 flex flex-col flex-grow m-2 p-4 space-y-2 rounded-xl bg-neutral-800">
            {playlist.tracks.map((track, index) => (
                <div className="flex flex-row" key={index}>
                    <Card className="bg-white flex flex-row px-4 py-2 justify-between w-4/5">
                        <p>{index + 1}</p>
                        <p>{track.name}</p>
                        <p>{toTimeString(track.duration_ms)}</p>
                    </Card>
                    <div className="items-center justify-center flex flex-row w-1/5">
                        <Button isIconOnly onPress={() => {swapTracks(index, index-1)}} variant="light">
                            <ChevronUpIcon className="w-8 h-8" />
                        </Button>
                        <Button isIconOnly onPress={() => swapTracks(index, index+1)} variant="light">
                            <ChevronDownIcon className="w-8 h-8" />
                        </Button>
                    </div>
                </div>
            ))}
        </div>
    )

    const instructions = (
        <div className="w-1/4 flex flex-col m-8 space-y-4 items-center">
            <p>Insert Instructions Here</p>
        </div>
    )

    const onClose = () => { setWarningOn(false) }

    return (
        <>
            <div className="flex h-full flex-row text-white">
                {playlistDisplay}
                {trackSelector}
                {instructions}
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