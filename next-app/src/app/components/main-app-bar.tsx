"use client";
import {
  Avatar,
  Button,
  Dropdown,
  DropdownItem,
  DropdownMenu,
  DropdownTrigger,
  Link,
  Navbar,
  NavbarBrand,
  NavbarItem,
  Tooltip,
} from "@heroui/react";
import { useRouter } from "next/navigation";
import { BugAntIcon } from "@heroicons/react/16/solid";
import { useAuth } from "@/contexts/AuthContext";
import { supabase } from "@/lib/supabase";

export default function MainAppBar() {
  const router = useRouter();
  const { user, loading, refetch } = useAuth();

  const handleSignOut = async () => {
    await supabase.auth.signOut();
    // Refetch auth state to update the UI immediately
    await refetch();
  };

  const handleSignIn = () => {
    router.push("/auth/login");
  };

  return (
    <Navbar className="text-white bg-black" maxWidth="full">
      <NavbarBrand>
        <Button
          onPress={() => router.push("/")}
          className="text-white"
          variant="bordered"
        >
          Playlist Knapsack
        </Button>
      </NavbarBrand>
      <NavbarItem>
        <Link
          href="https://github.com/kendale-hamilton/playlist-knapsack/issues"
          isExternal
        >
          <Tooltip content="Report a bug" className="text-white">
            <BugAntIcon className="w-4 h-4" />
          </Tooltip>
        </Link>
      </NavbarItem>
      {!loading && (
        <>
          {user && (
            <NavbarItem>
              <Dropdown>
                <DropdownTrigger>
                  <Avatar
                    name={user?.email?.charAt(0).toUpperCase()}
                    src={user?.user_metadata?.avatar_url}
                  />
                </DropdownTrigger>
                <DropdownMenu>
                  <DropdownItem
                    key="dashboard"
                    onPress={() => router.push("/dashboard")}
                  >
                    <p className="text-white">Dashboard</p>
                  </DropdownItem>
                  <DropdownItem key="Sign Out" onPress={() => handleSignOut()}>
                    <p className="text-white">Sign out</p>
                  </DropdownItem>
                </DropdownMenu>
              </Dropdown>
            </NavbarItem>
          )}
          {!user && (
            <NavbarItem>
              <Button onPress={handleSignIn}>Sign in</Button>
            </NavbarItem>
          )}
        </>
      )}
    </Navbar>
  );
}
