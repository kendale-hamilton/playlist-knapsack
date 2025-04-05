"use client"
import "./globals.css";
import { HeroUIProvider, ToastProvider } from "@heroui/react";
import React from "react";

export default function Providers({children}: {children: React.ReactNode}) {
    return (
        <HeroUIProvider>
            <ToastProvider />
            {children}
        </HeroUIProvider>
    )
}