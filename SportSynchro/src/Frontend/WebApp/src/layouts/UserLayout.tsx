import { Outlet } from "react-router-dom";
import UserNavbar from "../components/navigation/UserNavbar";

export default function UserLayout() {
  return (
    <div className="dark min-h-screen flex flex-col bg-gray-900 text-gray-200">
      <UserNavbar />

      <main className="flex-1 p-6">
        <Outlet />
      </main>
    </div>
  );
}
