"use client";
import getCookies from "@/app/helpers/get-cookies";
import toTimeString from "@/app/helpers/ms-convert";
import { FullPlaylist } from "@/types/Playlist"
import { Track } from "@/types/Track";
import { ChevronUpIcon, ChevronDownIcon } from "@heroicons/react/16/solid";
import { Card, Button } from "@nextui-org/react";
import { useEffect, useState } from "react";

type TracksProps = {
    playlist: FullPlaylist,
    setPlaylist: (playlist: FullPlaylist) => void
}

export default function Tracks(props: TracksProps) {
    const { playlist, setPlaylist } = props
    
    const swapTracks = (index_1: number, index_2: number) => {
        const newTracks: Track[] = [...playlist.tracks];
        [newTracks[index_1], newTracks[index_2]] = [newTracks[index_2], newTracks[index_1]];
        setPlaylist({ ...playlist, tracks: newTracks });
    }
    
    return (
        <div className="w-1/2 flex flex-col flex-grow m-2 p-4 space-y-2 rounded-xl bg-neutral-800">
            {playlist.tracks.map((track, index) => (
                <div className="flex flex-row" key={index}>
                    <Card className="bg-white flex flex-row px-4 py-2 justify-between w-4/5">
                        <p>{index + 1}</p>
                        <p>{track.name}</p>
                        <p>{toTimeString(track.duration_ms)}</p>
                    </Card>
                    <div className="items-center justify-center flex flex-row w-1/5">
                        { index > 0 && (
                            <Button isIconOnly onPress={() => {swapTracks(index, index-1)}} variant="light">
                                <ChevronUpIcon className="w-8 h-8" />
                            </Button>
                        )}
                        { index < playlist.tracks.length - 1 && (
                            <Button isIconOnly onPress={() => swapTracks(index, index+1)} variant="light">
                                <ChevronDownIcon className="w-8 h-8" />
                            </Button>
                        )}
                    </div>
                </div>
            ))}
        </div>
    )
}