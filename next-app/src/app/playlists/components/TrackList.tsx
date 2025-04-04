import { toTimeStringSeconds } from "@/app/helpers/time-functions";
import { Track } from "@/types/Track";
import { Card, Tooltip } from "@heroui/react";

type TrackListProps = {
    title: string,
    width: string,
    tracks: Track[],
    setPlaylist?: (playlist: Track[]) => void
}

export default function TrackList(props: TrackListProps) {
    const { title, width, tracks } = props;

    // const swapTracks = (index_1: number, index_2: number) => {
    //     const newTracks: Track[] = [...tracks];
    //     [newTracks[index_1], newTracks[index_2]] = [newTracks[index_2], newTracks[index_1]];
    //     setPlaylist && setPlaylist(newTracks);
    // }

    return (
        <div className={`flex flex-col p-8 space-y-4 items-center ${width}`}>
            <p className="font-bold underline">{title}</p>
            <div className="flex flex-col p-2 space-y-2 w-full">
                {tracks.map((track, index) => (
                    <div className="flex flex-row" key={index}>
                        <Tooltip content={<p className="text-white">Click to view on Spotify</p>} placement="top-end">
                            <Card className="bg-white flex flex-row px-4 py-2 justify-between w-full" isPressable onPress={() => window.open(track.spotify_url, "_blank")}>
                                <p className="w-10 justify-start">{index + 1}</p>
                                <p>{track.name}</p>
                                <p className="w-10">{toTimeStringSeconds(track.seconds)}</p>
                            </Card>
                        </Tooltip>
                        {/* {setPlaylist && (
                            <div className="items-center justify-center flex flex-row w-1/5">
                                {index > 0 && (
                                    <Button isIconOnly onPress={() => {swapTracks(index, index-1)}} variant="light">
                                        <ChevronUpIcon className="w-8 h-8" />
                                    </Button>
                                )}
                                { index < tracks.length - 1 && (
                                    <Button isIconOnly onPress={() => swapTracks(index, index+1)} variant="light">
                                        <ChevronDownIcon className="w-8 h-8" />
                                    </Button>
                                )}
                            </div>
                        )} */}
                    </div>
                ))}
            </div>
        </div>
    )
}