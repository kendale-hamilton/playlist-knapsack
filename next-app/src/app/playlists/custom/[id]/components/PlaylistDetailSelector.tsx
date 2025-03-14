import { playlistDuration, toTimeStringSeconds } from "@/app/helpers/time-functions"
import { FullPlaylist } from "@/types/Playlist"
import { Track } from "@/types/Track"
import { Button, Input, Image, Form } from "@nextui-org/react"
import { useState } from "react"

type PlaylistDetailSelectorProps = {
    width: string,
    id: string,
    tracks: Track[],
    desiredLength: number,
    setPlaylist: (playlist: FullPlaylist) => void,
    // setImage: (image: any) => void
}

export default function PlaylistDetailSelector(props: PlaylistDetailSelectorProps) {
    const { width, id, tracks, desiredLength, setPlaylist } = props
    const length = playlistDuration(tracks)
    // const handleFileChange = async (event: React.ChangeEvent<HTMLInputElement>) => {
    //     const file = event.target.files?.[0];
    //     if (file) {
    //         const reader = new FileReader();
    //         reader.readAsDataURL(file);
    //         reader.onload = () => {
    //             setImage(file);
    //         };
    //         reader.onerror = (error) => {
    //             console.error("Error reading file:", error);
    //         };
    //     }
    // };
      

    const placeholder = "https://pics.clipartpng.com/Brown_Backpack_PNG_Clip_Art-3025.png"
    const emptyPlaylist: FullPlaylist = {
        details: {
            name: "",
            seconds: length,
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

    return (
        <div className={`w-${width} flex flex-col m-2 p-8 space-y-4 items-center`}>
            <p className="font-bold underline">Enter Your Playlist Details</p>
            <Input label="Name" placeholder="Enter your new playlist name" type="text" value={newPlaylist.details.name} onValueChange={(value) => setName(value)} isRequired/>
            <Input label="Description" placeholder="Enter your new playlist description" type="text" value={newPlaylist.details.description} onValueChange={(value) => setDescription(value) } isClearable />
            {/*Figure out how to have the user upload an image */}
            {/* <Input type="file" label="Playlist Image" placeholder="Upload a custom image file" onChange={handleFileChange} /> */}
            <p>Length: {toTimeStringSeconds(length)}</p>
            <Button 
                type="submit" 
                color="primary" 
                onPress={() => {
                    setPlaylist(newPlaylist)
                    setNewPlaylist(emptyPlaylist)
                }} 
                isDisabled={!newPlaylist.details.name}
            >
                Save to Spotify
            </Button>
            { (length != desiredLength) && (
                <div className="flex flex-col items-center">
                    <p className="text-red-500">Warning:</p>
                    <p>A playlist of exactly length {toTimeStringSeconds(desiredLength)} could not be built.</p>
                </div>
            )}
        </div>
    )
}