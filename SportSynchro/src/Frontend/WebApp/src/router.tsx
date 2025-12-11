import { createBrowserRouter } from "react-router-dom";
import UserLayout from "./layouts/UserLayout";
import HomePage from "./pages/user/HomePage.tsx";
import AdminLayout from "./layouts/AdminLayout.tsx";
import AdminDashboardPage from "./pages/admin/AdminDashboardPage.tsx";
import SportsVisibilityPage from "./pages/admin/SportsVisibilityPage.tsx";
import LeagueVisibilityPage from "./pages/admin/LeagueVisibilityPage.tsx";
import AdminPreviewPage from "./pages/admin/AdminPreviewPage.tsx";
import SportDetailsPage from "./pages/user/SportDetailsPage.tsx";

export const router = createBrowserRouter([
  {
    element: <UserLayout />,
    children: [
      {
        path: "/",
        element: <HomePage />,
      },
      {
        path: "/sports/:id",
        element: <SportDetailsPage />,
      }
    ],
  },
  {
    element: <AdminLayout />,
    children: [
      { path: "/admin", element: <AdminDashboardPage /> },
      { path: "/admin/sports", element: <SportsVisibilityPage /> },
      { path: "/admin/sports/:sportId/leagues", element: <LeagueVisibilityPage /> },
      { path: "/admin/preview", element: <AdminPreviewPage /> },
    ],
  },
]);
