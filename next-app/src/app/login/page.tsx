"use client"
import { Button } from "@heroui/react";
import { signIn } from "../components/main-app-bar";

export default function LoginPage() {
    return (
        <div className="flex flex-col items-center p-8 gap-8 h-screen text-white">
            <p>Oops! You must be signed in to access this page.</p>
            <Button 
                onPress={() => signIn()}
            >
                Click here to sign in 
            </Button>
        </div>
    )
}