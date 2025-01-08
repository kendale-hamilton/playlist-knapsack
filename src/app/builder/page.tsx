"use client"

import { useSession } from "next-auth/react"

export default function Builder() {
    const session = useSession()
    console.log({session})
    return (
        <div className="text-purple-300">Welcome</div>
    )
}