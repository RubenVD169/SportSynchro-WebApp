import { NavLink } from "react-router-dom";

export default function UserNavbar() {
  return (
    <header className="flex items-center justify-between px-6 py-4 border-b border-gray-700 bg-gray-800 shadow-sm">
      <div className="text-xl font-semibold text-white">SportSynchro</div>

      <nav className="flex gap-6 text-gray-300">
        <NavLink
          to="/"
          className={({ isActive }) =>
            `transition-colors ${
              isActive
                ? "text-blue-400 font-medium"
                : "hover:text-blue-300 text-gray-300"
            }`
          }
        >
          Home
        </NavLink>

        <NavLink
          to="/logout"
          className={({ isActive }) =>
            `transition-colors ${
              isActive
                ? "text-blue-400 font-medium"
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
