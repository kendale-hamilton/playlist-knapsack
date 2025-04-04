import { Cookies } from "@/types/cookies";
import { deleteCookie, getCookie } from "cookies-next";

export default function getCookies(): Cookies {
    const isPreview = process.env.NEXT_PUBLIC_VERCEL_ENV === 'preview';
    if (!isPreview) {
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
    else {
        const cookieStore: Cookies = {
            userId: process.env.NEXT_PUBLIC_USER_ID,
            userDisplayName: process.env.NEXT_PUBLIC_USER_DISPLAY_NAME,
            userEmail: process.env.NEXT_PUBLIC_USER_EMAIL,
            userAvatar: process.env.NEXT_PUBLIC_USER_AVATAR,
            accessToken: process.env.NEXT_PUBLIC_ACCESS_TOKEN,
            refreshToken: process.env.NEXT_PUBLIC_REFRESH_TOKEN,
        };
        // console.log('cookieStore:', cookieStore);
        return cookieStore;
    }
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