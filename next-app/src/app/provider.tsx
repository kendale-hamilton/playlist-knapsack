"use client"
import "./globals.css";
import { NextUIProvider } from "@nextui-org/react";
import React from "react";

export default function Providers({children}: {children: React.ReactNode}) {
    return (
        <NextUIProvider>
            {children}
        </NextUIProvider>
    )
}