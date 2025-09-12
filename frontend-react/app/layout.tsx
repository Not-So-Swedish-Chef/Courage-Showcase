import type { Metadata } from "next";
import { Inter, Poppins } from "next/font/google";
import "./globals.css";
import {
  ClerkProvider,
  SignedIn,
  SignedOut,
  SignIn,
  SignInButton,
  SignUpButton,
  UserButton,
} from "@clerk/nextjs";

const poppins = Poppins({
  subsets: ["latin"],
  weight: ["400", "500", "600", "700"],
  variable: "--font-poppins",
});

export const metadata: Metadata = {
  title: "Event Hub",
  description: "Event Hub is a platform for event management.",
  icons: {
    icon: "/assets/images/logo.svg",
  },
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <ClerkProvider>
      <html lang="en">
        <body className={poppins.variable}>
          {children}
        </body>
      </html>
    </ClerkProvider>
  );
}
