import { NextRequest, NextResponse } from "next/server";
import { supabaseAdmin } from "@/lib/supabase-admin";

export async function GET(req: NextRequest) {
  const { searchParams } = new URL(req.url);
  const code = searchParams.get("code");
  const state = searchParams.get("state");
  const baseUrl = `${req.nextUrl.protocol}//${req.nextUrl.host}`;

  if (!code || !state) {
    return NextResponse.redirect(
      `${baseUrl}/error?message=Missing%20parameters`
    );
  }

  try {
    // Parse the state to get user ID and return URL
    let stateData;
    try {
      stateData = JSON.parse(state);
    } catch (e) {
      console.error("Invalid state parameter:", e);
      return NextResponse.redirect(
        `${baseUrl}/auth/login?message=Invalid%20request`
      );
    }

    const { userId, returnUrl } = stateData;

    if (!userId) {
      return NextResponse.redirect(
        `${baseUrl}/auth/login?message=User%20ID%20missing`
      );
    }

    // Exchange the authorization code for an access token
    const response = await fetch("https://accounts.spotify.com/api/token", {
      method: "POST",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
        Authorization: `Basic ${btoa(
          `${process.env.NEXT_PUBLIC_SPOTIFY_CLIENT_ID}:${process.env.SPOTIFY_CLIENT_SECRET}`
        )}`,
      },
      body: new URLSearchParams({
        grant_type: "authorization_code",
        code,
        redirect_uri: `${process.env.NEXT_PUBLIC_SPOTIFY_REDIRECT_URI}`,
      }),
    });

    const { access_token, refresh_token } = await response.json();

    const meRes = await fetch("https://api.spotify.com/v1/me", {
      headers: {
        Authorization: `Bearer ${access_token}`,
      },
    });

    const { display_name, email, id, images } = await meRes.json();

    // Check if user profile exists in Supabase
    const { data: existingProfile } = await supabaseAdmin
      .from("users")
      .select("*")
      .eq("id", userId)
      .single();

    if (existingProfile) {
      // Update existing profile with Spotify details
      const { error: updateError } = await supabaseAdmin
        .from("users")
        .update({
          display_name: display_name,
          avatar_url: images[0]?.url || null,
          spotify_user_id: id,
          spotify_access_token: access_token,
          spotify_refresh_token: refresh_token,
          updated_at: new Date().toISOString(),
        })
        .eq("id", userId);

      if (updateError) {
        console.error("Error updating user profile:", updateError);
        return NextResponse.redirect(
          `${baseUrl}/dashboard?error=Failed%20to%20update%20profile`
        );
      }
    } else {
      // Create new user profile
      const { error: insertError } = await supabaseAdmin.from("users").insert({
        id: userId,
        email: email,
        display_name: display_name,
        avatar_url: images[0]?.url || null,
        spotify_user_id: id,
        spotify_access_token: access_token,
        spotify_refresh_token: refresh_token,
      });

      if (insertError) {
        console.error("Error creating user profile:", insertError);
        return NextResponse.redirect(
          `${baseUrl}/dashboard?error=Failed%20to%20create%20profile`
        );
      }
    }

    // Redirect back to dashboard with success message
    return NextResponse.redirect(
      `${baseUrl}/dashboard?message=Spotify%20connected%20successfully`
    );
  } catch (error) {
    console.error("Failed to process Spotify callback:", error);
    return NextResponse.redirect(
      `${baseUrl}/dashboard?error=Failed%20to%20connect%20Spotify`
    );
  }
}
