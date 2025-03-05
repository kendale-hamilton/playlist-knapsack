"use client"
import { toSecs } from "@/app/helpers/ms-convert"
import { ChevronLeftIcon } from "@heroicons/react/16/solid"
import { Button, Input } from "@nextui-org/react"
import { useState } from "react"

type BuilderConfigurationProps = {
    onSubmit: (desiredLength: number, weightingFunction: Function) => void
}

type WeightingFunction = (index: number, playlistSize: number) => number

type WeightingSystem = {
    name: string,
    image: string,
    function: WeightingFunction
}

export default function BuilderConfiguration(props: BuilderConfigurationProps) {
    const { onSubmit } = props
    const [desiredLength, setDesireLength] = useState<string>("")

    const weightingSystems: WeightingSystem[] = [
        {name: "Unweighted", image: "/UnweightedGraph.png", function: (index, size) => 1  },
        {name: "Inverse", image: "/InverseGraph.png", function: (index, size) => 1 / (index + 1) },
        {name: "Linear", image: "/LinearGraph.png", function: (index, size) => size - index },
    ]
    const [weightingIndex, setWeightingIndex] = useState(0)

    const left = () => {
        const newIndex = weightingIndex == 0 ?  weightingSystems.length -1 : weightingIndex - 1
        setWeightingIndex(newIndex)
    }
    const right = () => {
        const newIndex = weightingIndex == weightingSystems.length - 1 ? 0 : weightingIndex + 1
        setWeightingIndex(newIndex)
    }

    return (
        <div className="w-1/4 flex flex-col m-8 space-y-4 items-center">
            <p className="font-bold underline">Configuration</p>
            <p>Select your desired weighting scheme below</p>
            <p>(x: track number, y: preference in final playlist) </p>
            <div className="flex flex-row space-x-4 items-center justify-center">
                <Button isIconOnly onPress={() => left() } variant="light">
                    <ChevronLeftIcon className="w-8 h-8" />
                </Button>
                <p className="w-24 text-center">{weightingSystems[weightingIndex].name}</p>
                <Button isIconOnly onPress={() => right()} variant="light">
                    <ChevronLeftIcon className="w-8 h-8 rotate-180" />
                </Button>
            </div>
            <img className="w-64 h-64" src={weightingSystems[weightingIndex].image} />
            <Input
                value={desiredLength}
                onValueChange={setDesireLength}
                isRequired
                label="Desired Time"
                pattern="^([0-1]?[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$"
                endContent={
                    <div className="pointer-events-none flex items-center">
                        <span className="text-default-400 text-small">hh:mm:ss</span>
                    </div>
                }
            />
            <Button color="primary" isDisabled={!desiredLength.length} onPress={() => onSubmit(toSecs(desiredLength), weightingSystems[weightingIndex].function)}>Submit Playlist</Button>
        </div>
    )
}