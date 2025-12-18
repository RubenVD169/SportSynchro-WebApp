import { useEffect, useRef } from "react";
import { useAuth } from "react-oidc-context";
import UserLayout from "../layouts/UserLayout";
import { FullscreenLoader } from "../components/ui/FullscreenLoader";

export default function RequireUser() {
  const { isAuthenticated, isLoading, signinRedirect } = useAuth();
  const hasRedirected = useRef(false);

  useEffect(() => {
    if (!isLoading && !isAuthenticated && !hasRedirected.current) {
      hasRedirected.current = true;
      signinRedirect();
    }
  }, [isLoading, isAuthenticated, signinRedirect]);

  if (isLoading) {
    return <FullscreenLoader text="Loading…" />;
  }

  if (!isAuthenticated) {
    return <FullscreenLoader text ="Redirecting to sign-in page…" />;
  }

  return <UserLayout />;
}
