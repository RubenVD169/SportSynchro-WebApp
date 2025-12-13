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

export const router = createBrowserRouter([
  {
    path: "/auth/callback",
    element: <AuthCallback />,
  },
  {
    element: <RequireUser />,
    children: [
      { path: "/", element: <HomePage /> },
      { path: "/sports/:id", element: <SportDetailsPage /> },
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
