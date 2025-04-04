"use client"
import { toTimeStringSeconds } from "@/app/helpers/time-functions"
import { FullPlaylist } from "@/types/Playlist"
import { ArrowsRightLeftIcon } from "@heroicons/react/16/solid"
import { Button, Image } from "@heroui/react"

type PlaylistDetailsProps = {
    onSwitch: () => void,
    playlist: FullPlaylist,
    width: string
}

export default function PlaylistDetails(props: PlaylistDetailsProps) {
    const { onSwitch, playlist, width } = props
    return (
        <div className={`w-${width} flex flex-col m-2 p-8 space-y-4 items-center`}>
            <div className="font-bold underline">Selected Playlist</div>
            <Image alt="Playlist Cover" className="w-32 h-32" src={playlist.details.images[0].url} />
            <p>{playlist.details.name}</p>
            <p>Length: {toTimeStringSeconds(playlist.details.seconds)}</p>
            <Button color="primary" onPress={onSwitch}>
                <p>Switch Playlist</p>
                <ArrowsRightLeftIcon className="w-6 h-6"/>
            </Button>
        </div>
    )
}