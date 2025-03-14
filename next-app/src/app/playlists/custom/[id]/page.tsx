"use client";
import getCookies from "@/app/helpers/cookie-functions";
import { Track } from "@/types/Track";
import { useEffect, useState } from "react";
import TrackList from "../../components/TrackList";
import PlaylistDetailSelector from "./components/PlaylistDetailSelector";
import { FullPlaylist } from "@/types/Playlist";
import { Button, Link, Modal, ModalBody, ModalContent, ModalFooter, ModalHeader } from "@nextui-org/react";
import { useSearchParams, useRouter } from "next/navigation";


export default function CustomPlaylist({params}: any){
    const searchParams = useSearchParams()
    const desiredLength = searchParams.get("desired-length")

    const router = useRouter()
    
    const [id, setId] = useState<string>("")
    const [tracks, setTracks] = useState<Track[]>()
    const [playlist, setPlaylist] = useState<FullPlaylist>()
    const [url, setUrl] = useState<string | null>()
    const [open, setOpen] = useState(false)
    // const [image, setImage] = useState<string | null>()

    useEffect(() => {
        if(!tracks){
            const fetchCustomPlaylist = async () => {
                const { id } = await params;
                setId(id)
                const cookies = getCookies();
                const response = await fetch(`/api/knapsack/users/${cookies.userId}/playlists/${id}`)
                const customPlaylist = await response.json()
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
                const body = {
                    playlist: playlist,
                    // image: btoa(image || "")
                }
                console.log("Posting playlist: ", body)
                const response = await fetch(`/api/spotify/users/${cookies.userId}/playlists`, {
                    method: "POST",
                    headers: {
                        Authorization: `Bearer ${cookies.accessToken}`,
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(body)
                })
                const newPlaylist = await response.json()
                setUrl(newPlaylist.url)
                setOpen(true)
            }
            postSpotifyPlaylist()
        }
    }, [playlist])

    if (!tracks) {
        return <div>Loading...</div>
    }

    return (
        <div className="flex flex-row text-white bg-neutral-900">
            <div className="hidden md:flex flex-row w-full">
                <PlaylistDetailSelector width="1/2" id={id} tracks={tracks} desiredLength={Number(desiredLength)} setPlaylist={setPlaylist} />
                <TrackList title="Selected Tracks" tracks={tracks} width="1/2" />
            </div>
            <div className="flex flex-col md:hidden w-full">
                <PlaylistDetailSelector width="full" id={id} tracks={tracks} desiredLength={Number(desiredLength)} setPlaylist={setPlaylist} />
                <TrackList title="Selected Tracks" tracks={tracks} width="full" />
            </div>
            <Modal isOpen={open} onClose={() => setOpen(false)}>
                <ModalContent>
                    <div className="text-white">
                        <ModalHeader>Your playlist has been uploaded to spotify!</ModalHeader>
                        <ModalBody>
                            <Link href={url ?? '/'}>Click here to view your playlist on spotify</Link>
                        </ModalBody>
                        <ModalFooter>
                            <Button color="primary" onPress={() => {router.push('/')}}>Home</Button>
                        </ModalFooter>
                    </div>
                </ModalContent>
            </Modal>
        </div>
    )

}