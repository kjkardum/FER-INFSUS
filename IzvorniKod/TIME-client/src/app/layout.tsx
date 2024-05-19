import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";
import { ReactNode } from "react";
import AuthenticationContextProvider from "@/contexts/authentication/AuthenticationContextProvider";

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "TIME",
  description: "Business time management",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: ReactNode;
}>) {
  return (
    <AuthenticationContextProvider>
      <html lang="en">
        <body className={inter.className}>{children}</body>
      </html>
    </AuthenticationContextProvider>
  );
}
