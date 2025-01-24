import { Cookies } from "@/types/cookies";
import { getCookie } from "cookies-next";

export default function getCookies(): Cookies {
    const cookieStore: Cookies = {
        userId: getCookie('userId') as string | undefined,
        userDisplayName: getCookie('userDisplayName') as string | undefined,
        userEmail: getCookie('userEmail') as string | undefined,
        userAvatar: getCookie('userAvatar') as string | undefined,
        accessToken: getCookie('accessToken') as string | undefined,
        refreshToken: getCookie('refreshToken') as string | undefined,
    };
    return cookieStore;
}