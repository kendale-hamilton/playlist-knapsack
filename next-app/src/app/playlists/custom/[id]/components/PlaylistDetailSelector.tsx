import { playlistDuration, toTimeStringSeconds } from "@/app/helpers/time-functions"
import { FullPlaylist } from "@/types/Playlist"
import { Track } from "@/types/Track"
import { Button, Input, Image, Form } from "@nextui-org/react"
import { useState } from "react"

type PlaylistDetailSelectorProps = {
    id: string,
    tracks: Track[],
    setPlaylist: (playlist: FullPlaylist) => void
}

export default function PlaylistDetailSelector(props: PlaylistDetailSelectorProps) {
    const { id, tracks, setPlaylist } = props
    const placeholder = "https://pics.clipartpng.com/Brown_Backpack_PNG_Clip_Art-3025.png"
    const emptyPlaylist: FullPlaylist = {
        details: {
            name: "",
            seconds: playlistDuration(tracks),
            images: [{
                url: placeholder,
                width: 0,
                height: 0
            }],
            id: id,
            description: ""
        },
        tracks: tracks
    }
    const [newPlaylist, setNewPlaylist] = useState<FullPlaylist>(emptyPlaylist)
    
    const setName = (name: string) => {
        setNewPlaylist({
            ...newPlaylist,
            details: {
                ...newPlaylist.details,
                name: name
            }
        })
    }

    const setDescription = (description: string) => {
        setNewPlaylist({
            ...newPlaylist,
            details: {
                ...newPlaylist.details,
                description: description
            }
        })
    }

    const setImage = (url: string) => {
        setNewPlaylist({
            ...newPlaylist,
            details: {
                ...newPlaylist.details,
                images: [{
                    url: url,
                    width: 128,
                    height: 128
                }]
            }
        })
    }

    return (
        <div className="w-1/4 flex flex-col m-2 p-4 space-y-4 items-center">
            <div className="bg-white rounded-xl">
                <Image src={newPlaylist.details.images[0].url || placeholder} width={128} height={128} />
            </div>
            <Input label="Name" placeholder="Enter your new playlist name" type="text" value={newPlaylist.details.name} onValueChange={(value) => setName(value)} isRequired/>
            <Input label="Description" placeholder="Enter your new playlist description" type="text" value={newPlaylist.details.description} onValueChange={(value) => setDescription(value) } isClearable />
            {/* TODO: Pick a new placeholder image and have user select an image file*/}
            <Input type="url" label="Playlist Image" placeholder="Enter the url of your new playlist's cover image" onValueChange={(value) => setImage(value)} />
            <p>Length: {toTimeStringSeconds(newPlaylist.details.seconds)}</p>
            <Button type="submit" color="primary" onPress={() => setPlaylist(newPlaylist)} isDisabled={!newPlaylist.details.name} >Save to Spotify</Button>
        </div>
    )
}