import { createBrowserRouter } from "react-router-dom";
import RequireUser from "./auth/RequireUser";
import RequireAdmin from "./auth/RequireAdmin";
import HomePage from "./pages/user/HomePage";
import SportDetailsPage from "./pages/user/SportDetailsPage";
import AdminDashboardPage from "./pages/admin/AdminDashboardPage";
import SportsVisibilityPage from "./pages/admin/SportsVisibilityPage";
import LeagueVisibilityPage from "./pages/admin/LeagueVisibilityPage";
import AdminPreviewPage from "./pages/admin/AdminPreviewPage";
import AuthCallback from "./auth/AuthCallBack";
import { RegisterPage } from "./pages/auth/RegisterPage";
import LogoutCallback from "./auth/LogoutCallBack";
import PaymentSuccessPage from "./pages/user/PaymentSuccesPage";
import PaymentCancelPage from "./pages/user/PaymentCancelPage";
import LeagueDetailsPage from "./pages/user/LeagueDetailsPage";

export const router = createBrowserRouter([
  {
    path: "/auth/callback", element: <AuthCallback />,
  },
  {
    path: "/auth/logout-callback", element: <LogoutCallback />,
  },
  {
    path: "/register", element: <RegisterPage />,
  },
  {
    element: <RequireUser />,
    children: [
      { path: "/", element: <HomePage /> },
      { path: "/sports/:id", element: <SportDetailsPage /> },
      { path: "/payment/success", element: <PaymentSuccessPage /> },
      { path: "/payment/cancel", element: <PaymentCancelPage /> },
      { path: "/sports/:sportId/:leagueId", element: <LeagueDetailsPage /> }
    ],
  },
  {
    element: <RequireAdmin />,
    children: [
      { path: "/admin", element: <AdminDashboardPage /> },
      { path: "/admin/sports", element: <SportsVisibilityPage /> },
      { path: "/admin/sports/:sportId/leagues", element: <LeagueVisibilityPage /> },
      { path: "/admin/preview", element: <AdminPreviewPage /> },
    ],
  },
]);
