"use client"
import { Button, Navbar, NavbarBrand, NavbarItem } from "@nextui-org/react";
import { signIn, signOut, useSession } from "next-auth/react";
import { useRouter } from "next/navigation";

export default function MainAppBar() {
    const router = useRouter();
    const session = useSession();

    return (
        <Navbar 
            className="text-white bg-black"
            maxWidth="full"    
        >
            <NavbarBrand>Playlist Knapsack</NavbarBrand>
            {!session?.data?.user && (
                <NavbarItem>
                    <Button onPress={() => signIn()}>
                        Sign in
                    </Button>
                </NavbarItem>
            )}
            {session?.data?.user && (
                <NavbarItem>
                    <Button onPress={() => signOut()}>
                        Sign out
                    </Button>
                </NavbarItem>
            )}
        </Navbar>
    )
}