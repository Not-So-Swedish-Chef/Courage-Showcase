import Link from "next/link";
import React from "react";
import Image from "next/image";

const Footer = () => {
  return (
    <footer className="border-t">
      <div className="flex-center wrapper flex-between flex flex-col gap  text-center sm:flex-row">
        <Link href={"/"}>
          <Image
            src={"/assets/images/logosvg2.svg"}
            alt="logo"
            width={60}
            height={38}
          ></Image>
        </Link>
        <p>2025 All Rights reserved.</p>
      </div>
    </footer>
  );
};

export default Footer;
