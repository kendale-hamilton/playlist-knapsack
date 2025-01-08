import NextAuth from "next-auth";
import SpotifyProvider from "next-auth/providers/spotify";

const handler = NextAuth({
    providers: [
        SpotifyProvider({
            clientId: process.env.SPOTIFY_CLIENT_ID ?? "",
            clientSecret: process.env.SPOTIFY_CLIENT_SECRET ?? ""
        })
    ],
    callbacks: {
        async jwt({ token, account }) {
            if (account) {
                token.accessToken = account.access_token
            }
            console.log('creating token: ', token)
            return token
        },
        async session({session, token}) {
            session.user.token = token.accessToken as string;
            console.log("creating session: ", session)
            return session;
        }
    }
    
})

export { handler as GET, handler as POST}
