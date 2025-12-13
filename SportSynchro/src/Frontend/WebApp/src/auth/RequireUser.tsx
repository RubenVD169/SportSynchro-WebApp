import { useEffect } from "react";
import { useAuth } from "react-oidc-context";
import UserLayout from "../layouts/UserLayout";

export default function RequireUser() {
  const { isAuthenticated, isLoading, signinRedirect } = useAuth();

  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      signinRedirect();
    }
  }, [isLoading, isAuthenticated, signinRedirect]);

  if (isLoading) {
    return <div>Loading…</div>;
  }

  if (!isAuthenticated) {
    return <div>Redirecting to login…</div>;
  }

  return <UserLayout />;
}
