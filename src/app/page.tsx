"use client"
import { useSession } from "next-auth/react";

export default function Home() {
  const { data: session } = useSession();
  if (session) {
    return (
      <div>
        Welcome, {session.user?.name}!
      </div>
    )
  }
  return (
    <div>
      Welcome Visitor
    </div>
  )
}
