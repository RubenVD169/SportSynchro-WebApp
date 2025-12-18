import { useEffect } from "react";
import { useAuth } from "react-oidc-context";
import { useNavigate } from "react-router-dom";
import { FullscreenLoader } from "../components/ui/FullscreenLoader";

export default function LogoutCallback() {
  const auth = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    auth.signoutRedirect().then(() => {
      navigate("/", { replace: true });
    });
  }, [auth, navigate]);

  return <FullscreenLoader text="Signing you out…" />;
}
