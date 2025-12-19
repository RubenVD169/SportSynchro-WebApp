import type { UserManagerSettings } from "oidc-client-ts";

const appBaseUrl = import.meta.env.VITE_APP_BASE_URL;

export const authSettings: UserManagerSettings = {
  authority: import.meta.env.VITE_AUTH_AUTHORITY,
  client_id: import.meta.env.VITE_AUTH_CLIENT_ID,

  redirect_uri: `${appBaseUrl}/auth/callback`,
  post_logout_redirect_uri: `${appBaseUrl}/auth/logout-callback`,
  silent_redirect_uri: `${appBaseUrl}/auth/silent-renew`,

  response_type: "code",
  scope: "openid profile roles sportsynchro.api.read sportsynchro.api.write",
  monitorSession: true,
  automaticSilentRenew: true,
};