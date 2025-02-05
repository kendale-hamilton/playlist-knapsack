"use client"
import { Cookies } from "@/types/cookies";
import { Avatar, Button, Dropdown, DropdownItem, DropdownMenu, DropdownTrigger, Navbar, NavbarBrand, NavbarItem } from "@nextui-org/react";
import { redirect, useRouter } from "next/navigation";
import querystring from 'querystring';
import { useEffect, useState } from "react";
import getCookies from "../helpers/get-cookies";

export default function MainAppBar() {
    const router = useRouter();
    const [cookies, setCookies] = useState<Cookies | null>();

    useEffect(() => {
        const setCookieState = async () => {
            const cookieStore = getCookies();
            setCookies(cookieStore);
        }
        setCookieState()
    }, [])

    const signIn = async () => {
        const state = window.location.href;
        redirect('https://accounts.spotify.com/authorize?' +
            querystring.stringify({
                response_type: 'code',
                client_id: process.env.NEXT_PUBLIC_SPOTIFY_CLIENT_ID,
                scope: 'playlist-read-private playlist-read-collaborative playlist-modify-public playlist-modify-private user-read-private user-read-email',
                redirect_uri: 'http://localhost:3000/api/callback',
                state: state,
                show_dialog: true
            })
        );
    }

    const signOut = async () => {
        await fetch('/api/logout');
        setCookies({});
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
        </Navbar>
    )
}