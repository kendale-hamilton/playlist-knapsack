# Playlist Knapsack

### About the App
Consider a menu of many items and you want to know how to spend exactly $50. How could you efficiently calculate what items you must buy? Complicating this problem, what if you only wanted each item one time at most?

This is essentially the problem that Playlist Knapsack solves. This problem is otherwise refered to as the "Subset Sum" problem. The app solves the problem using an algotithm I learned in my Algorithms class at Hillsdale College, taught by Dr. Oliver Serang.

Soon, the algorithm will be updated to solve the "Knapsack" problem, where a user's preference (represented as a number between 0 and 1) for a song is considered when building the playlist.

### Use the app

Visit [Playlist Knapsack](https://www.playlist-knapsack.vercel.app) to use the app.
> **Disclaimer**: At the moment, the app can only be used by certain preconfigured Spotify accounts, due to the way Spotify allows access to their APIs. This restriction will be removed soon with Spotify's approval. In the meantime, you can follow the guide below to run the app locally using your own Spotify Web API app.

### Local use
After cloning the repo, you will need a `.env.local` file in the next-app folder, and a `local.settings.json` in the functionsApi folder. 

#### `.env.local` File

In your `.env.local file`, you will need the following settings:
```env
    NEXT_PUBLIC_SPOTIFY_CLIENT_ID="YOUR CLIENT ID"
    SPOTIFY_CLIENT_SECRET="YOUR CLIENT SECRET"
    NEXT_PUBLIC_SPOTIFY_REDIRECT_URI="http://localhost:7071/api/spotify/callback"
    NEXT_PUBLIC_BACKEND_URL="http://localhost:7071"
```

#### `local.settings.json` File

The following is an example of the `local.settings.json` that you will need:
```local
    {
        "IsEncrypted": false,
        "Values": {
            "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
            "SPOTIFY_CLIENT_ID": "YOUR CLIENT ID",
            "SPOTIFY_CLIENT_SECRET": "YOUR CLIENT SECRET",
            "SPOTIFY_REDIRECT_URI": "http://localhost:7071/api/spotify/callback",
            "FRONTEND_REDIRECT_URI": "http://localhost:3000",
            "AZURE_BLOB_CONNECTION_STRING": "YOUR BLOB STORAGE CONNECTION STRING"
        },
        "Host": {
            "CORS": "*"
        }
    }
```

You will also need Azure Functions Core Tools installed.

#### Running the App Locally

After your settings are configured, open two terminals and run:
    `npm run app`
    and
    `npm run api`

Navigate to http://localhost:3000 and begin testing the app!
