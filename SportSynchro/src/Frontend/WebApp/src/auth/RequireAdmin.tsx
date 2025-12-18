import { useAuth } from "react-oidc-context";
import { Navigate } from "react-router-dom";
import AdminLayout from "../layouts/AdminLayout";

export default function RequireAdmin() {
  const { isAuthenticated, isLoading, user } = useAuth();

  if (isLoading) return <div>Loading…</div>;

  if (!isAuthenticated) {
    return <Navigate to="/" replace />;
  }

  const roles = user?.profile?.roles ?? user?.profile?.role;
  const isAdmin = Array.isArray(roles)
    ? roles.includes("Admin")
    : roles === "Admin";

  if (!isAdmin) {
    return <Navigate to="/" replace />;
  }

  return <AdminLayout />;
}
