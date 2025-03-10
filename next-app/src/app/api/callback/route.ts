import { NextRequest, NextResponse } from "next/server";
import { cookies } from "next/headers";

export async function GET(req: NextRequest) {
    const { searchParams } = new URL(req.url);
    const code = searchParams.get('code');
    const state = searchParams.get('state');
    const baseUrl = `${req.nextUrl.protocol}//${req.nextUrl.host}`;
  
    if (!code || !state) {
      return NextResponse.redirect(`${baseUrl}/error?message=Missing%20parameters`);
    }
  
    try {
      // Exchange the authorization code for an access token
        const response = await fetch('https://accounts.spotify.com/api/token', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                Authorization: `Basic ${btoa(`${process.env.SPOTIFY_CLIENT_ID}:${process.env.SPOTIFY_CLIENT_SECRET}`)}`,
            },
            body: new URLSearchParams({
                grant_type: 'authorization_code',
                code,
                redirect_uri: 'http://localhost:3000/api/callback',
            }),
        });
    
        const { access_token, refresh_token } = await response.json();

        const meRes = await fetch('https://api.spotify.com/v1/me', {
            headers: {
                Authorization: `Bearer ${access_token}`,
            },
        });

        const { display_name, email, id, images } = await meRes.json();

        const cookieStore = await cookies();
        cookieStore.set('userId', id);
        cookieStore.set('userDisplayName', display_name);
        cookieStore.set('userEmail', email);
        cookieStore.set('userAvatar', images[0].url);
        cookieStore.set('accessToken', access_token);
        cookieStore.set('refreshToken', refresh_token);

        const res = NextResponse.redirect(`${baseUrl}`);

        return res;
    } catch (error) {
        console.error('Failed to fetch access token:', error);
        return NextResponse.redirect(`${baseUrl}/error?message=Token%20exchange%20failed`);
    }
  }