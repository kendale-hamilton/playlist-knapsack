"use client"
import { Divider, Card, CardBody, CardHeader, Image } from "@nextui-org/react";
import { useRouter } from "next/navigation";

export default function Home() {
  const router = useRouter();
  return (
    <div className="flex h-full justify-center items-center">
      <Card isPressable onPress={() => router.push('/builder')} className="bg-gray-500">
        <CardHeader className="flex gap-3">
          <Image
            alt="spotify logo"
            height={40}
            radius="sm"
            src="./spotify-svgrepo-com.svg"
            width={40}
          />
          <p> Get started with Playlist Knapsack</p>
        </CardHeader>
        <Divider />
        <CardBody>
          <p>Click to begin building your perfect playlist</p>
        </CardBody>
      </Card>
    </div>
  )
}
