import { toTimeStringSeconds } from "@/app/helpers/ms-convert";
import { Track } from "@/types/Track";
import { ChevronUpIcon, ChevronDownIcon } from "@heroicons/react/16/solid";
import { Button, Card } from "@nextui-org/react";

type TrackListProps = {
    w: string,
    tracks: Track[],
    setPlaylist?: (playlist: Track[]) => void
}

export default function TrackList(props: TrackListProps) {
    const { w, tracks, setPlaylist } = props;

    const swapTracks = (index_1: number, index_2: number) => {
        const newTracks: Track[] = [...tracks];
        [newTracks[index_1], newTracks[index_2]] = [newTracks[index_2], newTracks[index_1]];
        setPlaylist && setPlaylist(newTracks);
    }

    return (
        <div className={`flex flex-col space-y-2 rounded-xl ${w}`}>
            {tracks.map((track, index) => (
                <div className="flex flex-row" key={index}>
                    <Card className="bg-white flex flex-row px-4 py-2 justify-between w-4/5">
                        <p>{index + 1}</p>
                        <p>{track.name}</p>
                        <p>{toTimeStringSeconds(track.seconds)}</p>
                    </Card>
                    {setPlaylist && (
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
                    )}
                </div>
            ))}
        </div>
    )
}