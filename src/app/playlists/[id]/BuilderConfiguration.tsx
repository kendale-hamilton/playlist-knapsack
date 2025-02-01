"use client"
import { ChevronLeftIcon } from "@heroicons/react/16/solid"
import { Button, Input } from "@nextui-org/react"
import { useState } from "react"

type BuilderConfigurationProps = {
    onSubmit: (desiredLength: string, weightingFunction: Function) => void
}

export default function BuilderConfiguration(props: BuilderConfigurationProps) {
    const { onSubmit } = props
    const [desiredLength, setDesireLength] = useState<string>("")

    const weightingSystems = [
        {name: "Unweighted", image: "/UnweightedGraph.png", function: () => console.log("Unweighted") },
        {name: "Inverse", image: "/InverseGraph.png", function: () => console.log("Inverse") },
        {name: "Linear", image: "/LinearGraph.png", function: () => console.log("Linear") },
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
            <Button color="primary" isDisabled={!desiredLength.length} onPress={() => onSubmit(desiredLength ?? "", weightingSystems[weightingIndex].function)}>Submit Playlist</Button>
        </div>
    )
}