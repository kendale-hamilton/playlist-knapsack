import { NextRequest, NextResponse } from "next/server";
import { cookies } from "next/headers";

export async function GET(req: NextRequest) {
    const baseUrl = `${req.nextUrl.protocol}//${req.nextUrl.host}`;

    const cookieStore = await cookies();

    cookieStore.set('userId', '');
    cookieStore.set('userDisplayName', '');
    cookieStore.set('userEmail', '');
    cookieStore.set('userAvatar', '');
    cookieStore.set('accessToken', '');
    cookieStore.set('refreshToken', '');

    return NextResponse.redirect(baseUrl);
}