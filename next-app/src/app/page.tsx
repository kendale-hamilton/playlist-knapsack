"use client"
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import getCookies from "./helpers/cookie-functions";
import { Cookies } from "@/types/cookies";
import { Card, CardHeader, Divider, Image, Link } from "@nextui-org/react";
import { signIn } from "./components/main-app-bar";

export default function Home() {
  const router = useRouter();
  const [cookies, setCookies] = useState<Cookies | null>();
  const [loading, setLoading] = useState<boolean>(true);

  useEffect(() => {
    const setCookieState = async () => {
      const cookieStore = getCookies();
      setCookies(cookieStore);
    }
    setCookieState()
    setLoading(false);
  }, [])

  return (
    <div className="flex flex-col bg-neutral-900 gap-6 p-8 text-white w-full overflow-x-scroll">
      <div className ="items-center justify-center flex flex-col gap-4">
        <p className="text-4xl font-bold underline">Welcome to Playlist Knapsack</p>
        <p>Keep reading to learn about the app, or click below to get started!</p>
        {!loading && (
          <>
          {cookies?.userDisplayName && (
            <Card isPressable onPress={() => router.push('/playlists')} className="bg-gray-500">
              <CardHeader className="p-4 gap-2">
                <Image
                  alt="spotify logo"
                  height={40}
                  radius="sm"
                  src="./spotify-svgrepo-com.svg"
                  width={40}
                />
                <p className="font-bold">Get Started!</p>
              </CardHeader>
            </Card>
          )}
          {!cookies?.userDisplayName && (
            <Card isPressable onPress={() => signIn()} className="bg-gray-500">  
              <CardHeader className="p-4 gap-2">
                <Image
                  alt="spotify logo"
                  height={40}
                  radius="sm"
                  src="./spotify-svgrepo-com.svg"
                  width={40}
                />
                <p className="font-bold">Sign In</p>
              </CardHeader>
            </Card>
          )}
          </>
        )}
      </div>
      <Divider />
      <div className="flex flex-col items-start px-8 md:px-32">
        <p className="text-center font-bold">DISCLAIMER:</p>
        <p className="italic">This app is not available for public use until Spotify approves the API Extension Request for it. In the meantime, users&apos; spotify accounts must be added manually. Please reach out to me (contact info below) if you would like to be added.</p>
        <p className="underline font-bold">The Algorithm:</p>
        <div className="flex flex-col gap-2 p-4">
          <p>Consider a menu of many items and you want to know how to spend exactly $50. How could you efficiently calculate what items you must buy? Complicating this problem, what if you only wanted each item one time at most? </p>
          <p>This is essentially the problem that Playlist Knapsack solves. To solve this problem, which is called the &quot;subset sum&quot; problem, the algorithm represents each track in the playlist as a polynomial. For example, a song that is 120 seconds long is represented as: </p>
        <div className="flex flex-row justify-center items-center"><p className="text-center">1 + x</p><sup>120</sup></div>
        <p className="text-center">or</p>
        <div className="flex flex-row justify-center items-center"><p>1x</p><sup>0</sup><p> + x</p><sup>120</sup></div>
        <p>Now, consider what happens if you were to multiply a two of these polynomials together: </p>
        <div className="flex flex-row justify-center items-center">
          <p className="text-center">(1 + 1x</p><sup>120</sup><p>)</p>
          <p className="p-4">x</p>
          <p> (1 + 1x</p><sup>157</sup><p>)</p>
          <p className="p-4"> = </p>
          <p>1 + 1x</p><sup>120</sup><p> + 1x</p><sup>157</sup><p> + 1x</p><sup>277</sup>
        </div>
        <div>
          <span>Since multiplying numbers with exponents is the same as adding their exponents, the 1x</span><sup>0</sup><span> can be understood as adding neither song to the new playlist. 1x</span><sup>120</sup><span> can be understood as adding the first song, but not the second, to the playlist. 1x</span><sup>157</sup><span> can be understood as the opposite.</span>
          <span>Lastly, 1x</span><sup>277</sup><span> can be understood as adding both songs to the playlist. By multiplying each track&apos;s polynomial together, we end with a polynomial that encodes all possible total durations (with each song included at most once) where the totals are exponents that have a nonzero value as their coefficient. </span>
        </div>
        <div>
          <span>Now, we simply need to check if this final polynomial has a positive coefficient for x</span><sup>t</sup><span>, where t is our desired total. Importantly, all of the polynomials that were multiplied iteratively to produce this polynomial are still needed.</span>
        </div>
        <p>Next, we must figure out how to get from t back to which individual songs where included to build this total. This is called the &quot;backwards pass&quot;. To accomplish this, we create a new polynomial: </p>
        <div className="flex flex-row justify-center items-center"><p>1 + x</p><sup>t</sup></div>
        <p>and seperately subtract each of the two factors that formed the final product polynomial. These factors are refered to as the left and right polynomials. The new polynomial minus the left gives us &quot;leftover right&quot; and vice versa. Then, a match in the left polynomial and leftover left tell us which component of left was used to get to our desired total. The same is true for the right side.</p>
        <div>
          <span>This process can be done iteratively until we know, for each track&apos;s polynomial, whether 1x</span><sup>0</sup><span>or the 1x</span><sup>length</sup><span> was used to get to the desired total. If 1x</span><sup>0</sup><span> was used, then the song was not used to get to the total. If the exponent with the songs length was used, then the song was used. We can then add all of the necessary songs to a new playlist, and it will be of length t.</span>
        </div>
        <div>
          {/*TODO: Confirm the runtime*/}
          <span>This process sounds simple, but multiplying massive polynomials is no quick task. In big O terms, naive polynomial multiplication is O(n</span><sup>2</sup><span>). However, using Fast Fourier Transform, this can be cut down to O(n log(n)), where n is the number of terms of in the polynomial. Using FFT and Inverse FFT to quickly multiply and subtract polynomials, this algorithm has a final runtime of O(nk log(nk) log(n)), where n is the size of the original playlist and k is the ?.</span>
        </div>
      </div>
      <p className="underline font-bold">Upcoming Features:</p>
      <div className="flex flex-col gap-2 p-4">
        <p>1. The above algorithm can be modified to consider the user&apos;s preference for each song. Using the same polynomial method, but replacing coeffecients of 1 with a value between 0 and 1 representing how much the user wants to include it, songs can be selected based on preference by using Max Convolution during the polynomial multiplication step.</p>
        <p>2. While far less of the music streaming marketshare, Apple Music and other platforms also provide similar web apis. In the future, the app could easily be expanded to allow a user to sign in with any of these platforms.</p>
        <p>3. Playlists created by users could be saved to allow users to easily share a playlist of a specific duration with others. This would be most useful if a description or tag could be added to each new playlist that explains the use case. For example, if I created a playlist that is exactly as long as the drive from Columbus, Ohio to Saint Louis, Missouri, users making the same drive could easily find the playlist.</p>
        <p>4. Running the algorithm multiple times on smaller chunks of the playlist would allow the user to insert songs that they want to play at specific times in their playlist. For example, if they enter West Virignia exactly 1 hour and 42 minutes into their drive, they could force Country Roads to play at that time in their playlist, while still having the full playlist built using the app.</p>
        <p>5. Most ambitiously, if the playlist could be played on a player built into the app, GPS location could be used to automatically insert or remove songs on the fly to ensure that certain songs play at the designated location.</p>
      </div>
      <p className="underline font-bold">About the Project:</p>
      <div className="flex flex-col gap-2 p-4">
        <p>Playlist Knapsack was built using NextJS and Azure Functions in C# and is deployed in an Azure Static Web App.</p>
        <div className="flex flex-col gap-4 items-center md:flex-row md:items-start">
          <div className="flex flex-col gap-2">
            <p>It was created by Kendale Hamilton, a Senior studying Politics and Computer Science at Hillsdale College, to graduate in May 2025.</p>
            <div className="flex flex-row gap-8 px-8">
              <Link href="mailto:kendaleernest@gmail.com" isExternal>Email</Link>
              <Link href="https://www.linkedin.com/in/kendale-hamilton-b72563265/" isExternal>LinkedIn</Link>
              <Link href="https://github.com/kendale-hamilton" isExternal>GitHub</Link>
            </div>
          </div>
          <Image alt="Kendale Pic" src="./kendale-headshot.jpg"/>
        </div>
      </div>
      </div>
    </div>
  )
}
