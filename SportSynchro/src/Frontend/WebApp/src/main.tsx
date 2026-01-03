import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import { RouterProvider } from "react-router-dom";
import { router } from "./router";
import { SportsProvider } from './contexts/SportsContext';
import { AuthProvider } from "react-oidc-context";
import { authSettings } from "./config/authconfig";
import AuthAxiosBridge from './auth/AuthAxiosBridge';
import "simplebar-react/dist/simplebar.min.css";

const onSigninCallback = (): void => {
  window.history.replaceState({}, document.title, window.location.pathname);
};

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <AuthProvider {...authSettings} onSigninCallback={onSigninCallback}>
      <AuthAxiosBridge />
    <SportsProvider>
      <RouterProvider router={router} />
    </SportsProvider>
    </AuthProvider>
  </StrictMode>,
)
