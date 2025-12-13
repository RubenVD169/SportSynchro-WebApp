import { useAuth } from "react-oidc-context";
import { useEffect, useRef } from "react";
import { useNavigate } from "react-router-dom";
import { hasRole } from "./hasRole";

export default function AuthCallback() {
  const { user, isAuthenticated, isLoading } = useAuth();
  const navigate = useNavigate();
  const hasNavigated = useRef(false);

  useEffect(() => {
    if (isLoading || !isAuthenticated || hasNavigated.current) {
      return;
    }

    hasNavigated.current = true;

    if (hasRole(user, "Admin")) {
      navigate("/admin", { replace: true });
    } else {
      navigate("/", { replace: true });
    }
  }, [isLoading, isAuthenticated, user, navigate]);

  return <div>Signing you in…</div>;
}
