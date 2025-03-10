import { Cookies } from "@/types/cookies";
import { deleteCookie, getCookie } from "cookies-next";

export default function getCookies(): Cookies {
    const cookieStore: Cookies = {
        userId: getCookie('userId') as string | undefined,
        userDisplayName: getCookie('userDisplayName') as string | undefined,
        userEmail: getCookie('userEmail') as string | undefined,
        userAvatar: getCookie('userAvatar') as string | undefined,
        accessToken: getCookie('accessToken') as string | undefined,
        refreshToken: getCookie('refreshToken') as string | undefined,
    };
    console.log('cookieStore:', cookieStore);
    return cookieStore;
}

export function clearCookies() {
    deleteCookie('userId');
    deleteCookie('userDisplayName');
    deleteCookie('userEmail');
    deleteCookie('userAvatar');
    deleteCookie('accessToken');
    deleteCookie('refreshToken');
    return getCookies();
}