import type { UserManagerSettings } from "oidc-client-ts";

export const authSettings: UserManagerSettings = {
  authority: "https://localhost:5001",
  client_id: "webapp-client",
  redirect_uri: "http://localhost:5173/auth/callback",
  post_logout_redirect_uri: "http://localhost:5173/",
  silent_redirect_uri: "http://localhost:5173/auth/silent-renew",
  response_type: "code",
  scope: "openid profile roles sportsynchro.api.read sportsynchro.api.write",
  monitorSession: true,
  automaticSilentRenew: true,
};
