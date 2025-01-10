import NextAuth from "next-auth";
import SpotifyProvider from "next-auth/providers/spotify";

const handler = NextAuth({
    providers: [
        SpotifyProvider({
            clientId: process.env.SPOTIFY_CLIENT_ID ?? "",
            clientSecret: process.env.SPOTIFY_CLIENT_SECRET ?? "",
            authorization: {
                params: {
                    scope: "playlist-read-private playlist-read-collaborative playlist-modify-public playlist-modify-private user-read-private user-read-email"
                }
            }
        })
    ],
    callbacks: {
        async jwt({ token, account }) {
            if (account) {
                token.userId = account.id
                token.accessToken = account.access_token
            }
            console.log('creating token: ', token)
            return token
        },
        async session({session, token}) {
            session.user.id = token.userId as string;
            session.user.token = token.accessToken as string;
            console.log("creating session: ", session)
            return session;
        }
    }
    
})

export { handler as GET, handler as POST}
