import toTimeString from "@/app/helpers/ms-convert"
import { FullPlaylist } from "@/types/Playlist"
import { ArrowsRightLeftIcon } from "@heroicons/react/16/solid"
import { Button } from "@nextui-org/react"

type PlaylistDetailsProps = {
    playlist: FullPlaylist,
    setWarningOn: (warning: boolean) => void
}

export default function PlaylistDetails(props: PlaylistDetailsProps) {
    const { playlist, setWarningOn } = props
    return (
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
}