import { Outlet } from "react-router-dom";
import AdminNavbar from "../components/navigation/AdminNavbar";

export default function AdminLayout() {
    return (
        <div className="dark min-h-screen flex flex-col bg-gray-950 text-gray-200">
            <AdminNavbar />
            <main className="flex-1 p-6">
                <Outlet />
            </main>
        </div>
    );
}