"use client"
import { Cookies } from "@/types/cookies";
import { Avatar, Button, Dropdown, DropdownItem, DropdownMenu, DropdownTrigger, Link, Navbar, NavbarBrand, NavbarItem, Tooltip } from "@nextui-org/react";
import { redirect, useRouter } from "next/navigation";
import querystring from 'querystring';
import { useEffect, useState } from "react";
import getCookies, { clearCookies } from "../helpers/cookie-functions";
import { BugAntIcon } from "@heroicons/react/16/solid";

export const signIn = async () => {
    const state = window.location.href;
    redirect('https://accounts.spotify.com/authorize?' +
        querystring.stringify({
            response_type: 'code',
            client_id: process.env.NEXT_PUBLIC_SPOTIFY_CLIENT_ID,
            scope: 'playlist-read-private playlist-read-collaborative playlist-modify-public playlist-modify-private user-read-private user-read-email ugc-image-upload',
            redirect_uri: 'http://localhost:7071/api/spotify/callback',
            state: state,
            show_dialog: true
        })
    );
}

export default function MainAppBar() {
    const router = useRouter();
    const [cookies, setCookies] = useState<Cookies | null>();
    const [loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        const setCookieState = async () => {
            const cookieStore = getCookies();
            setCookies(cookieStore);
        }
        setCookieState()
        setLoading(false);
    }, [])

    const signOut = async () => {
        const emptyStore = await clearCookies();
        setCookies(emptyStore);
        if (window.location.pathname === "/") {
            window.location.reload();
        } else {
            router.push("/");
        }
    }

    return (
        <Navbar 
            className="text-white bg-black"
            maxWidth="full"    
        >
            <NavbarBrand>
                <Button 
                    onPress={() => router.push("/")}
                    className="text-white"
                    variant="bordered"
                >
                    Playlist Knapsack
                </Button>
            </NavbarBrand>
            <NavbarItem>
                <Link href="https://github.com/kendale-hamilton/playlist-knapsack/issues" isExternal>
                    <Tooltip content="Report a bug" className="text-white">
                        <BugAntIcon className="w-4 h-4" />
                    </Tooltip>
                </Link>
            </NavbarItem>
            {!loading && (
                <>
                    {cookies?.userDisplayName && (
                        <NavbarItem>
                            <Dropdown>
                                <DropdownTrigger>
                                    <Avatar src={cookies?.userAvatar} />
                                </DropdownTrigger>
                                <DropdownMenu>
                                    <DropdownItem key="Sign Out" onPress={() => signOut()}>
                                        <p className="text-white">Sign out</p>
                                    </DropdownItem>
                                </DropdownMenu>
                            </Dropdown>
                        </NavbarItem>
                    )} 
                    {!cookies?.userDisplayName && (
                        <NavbarItem>
                            <Button onPress={() => signIn()}>
                                Sign in
                            </Button>
                        </NavbarItem>
                    )}
                </>
            )}  
        </Navbar>
    )
}