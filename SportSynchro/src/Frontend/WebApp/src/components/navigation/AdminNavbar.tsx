import { NavLink } from "react-router-dom";

export default function AdminNavbar() {
    return (
        <header className="flex items-center justify-between px-6 py-4 border-b border-gray-800 bg-gray-950 shadow-sm">
            <div className="text-xl font-semibold text-white">Admin Panel</div>
            <nav className="flex gap-6 text-gray-300 text-sm">
                <NavLink
                    to="/admin"
                    className={({ isActive }) =>
                        `uppercase tracking-wide transition-colors ${isActive
                            ? "text-blue-400 font-semibold"
                            : "hover:text-blue-300 text-gray-300"
                        }`
                    }
                >
                    Dashboard
                </NavLink>
                <NavLink
                    to="/admin/sports"
                    className={({ isActive }) =>
                        `uppercase tracking-wide transition-colors ${isActive
                            ? "text-blue-400 font-semibold"
                            : "hover:text-blue-300 text-gray-300"
                        }`
                    }
                >
                    Sports
                </NavLink>
                <NavLink
                    to="/admin/preview"
                    className={({ isActive }) =>
                        `uppercase tracking-wide transition-colors ${isActive
                            ? "text-blue-400 font-semibold"
                            : "hover:text-blue-300 text-gray-300"
                        }`
                    }
                >
                    Preview
                </NavLink>
                <NavLink
                    to="/logout"
                    className={({ isActive }) =>
                        `uppercase tracking-wide transition-colors ${isActive
                            ? "text-blue-400 font-semibold"
                            : "hover:text-blue-300 text-gray-300"
                        }`
                    }
                >
                    Logout
                </NavLink>
            </nav>
        </header>
    );
}
