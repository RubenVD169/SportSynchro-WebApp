import { useEffect } from "react";
import { useAuth } from "react-oidc-context";
import { api } from "../lib/api";

export default function AuthAxiosBridge() {
  const auth = useAuth();

  useEffect(() => {
    const interceptorId = api.interceptors.request.use((config) => {
      if (auth.user?.access_token) {
        config.headers.Authorization = `Bearer ${auth.user.access_token}`;
      }
      return config;
    });

    return () => {
      api.interceptors.request.eject(interceptorId);
    };
  }, [auth.user]);

  return null;
}
