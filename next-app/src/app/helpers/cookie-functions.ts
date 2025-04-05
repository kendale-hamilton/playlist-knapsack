import { Cookies } from "@/types/cookies";
import { deleteCookie, getCookie } from "cookies-next";

export default function getCookies(): Cookies {
    const cookieStore: Cookies = {
        userId: (getCookie('userId') as string | undefined) || process.env.NEXT_PUBLIC_USER_ID,
        userDisplayName: (getCookie('userDisplayName') as string | undefined) || process.env.NEXT_PUBLIC_USER_DISPLAY_NAME,
        userEmail: (getCookie('userEmail') as string | undefined) || process.env.NEXT_PUBLIC_USER_EMAIL,
        userAvatar: (getCookie('userAvatar') as string | undefined) || process.env.NEXT_PUBLIC_USER_AVATAR,
        accessToken: (getCookie('accessToken') as string | undefined) || process.env.NEXT_PUBLIC_ACCESS_TOKEN,
        refreshToken: (getCookie('refreshToken') as string | undefined) || process.env.NEXT_PUBLIC_REFRESH_TOKEN,
    };

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