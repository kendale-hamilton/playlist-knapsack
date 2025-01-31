import type { Metadata } from "next";
import "./globals.css";
import MainAppBar from "./components/main-app-bar";
import Providers from "./provider";

export const metadata: Metadata = {
  title: "Playlist Knapsack",
  description: "Generated by create next app",
};

export default async function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {

  return (
    <html lang="en" className="dark">
      <body className="bg-neutral-900">
        <Providers>
          <div className="flex flex-col h-screen">
            <MainAppBar />
            <div className="flex-grow" >
              {children}
            </div>
          </div>
        </Providers>
      </body>
    </html>
  );
}
