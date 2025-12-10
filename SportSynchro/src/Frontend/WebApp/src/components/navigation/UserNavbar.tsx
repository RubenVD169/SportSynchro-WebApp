import { NavLink } from "react-router-dom";

export default function UserNavbar() {
  return (
    <header className="flex items-center justify-between px-6 py-4 border-b border-gray-300 bg-white shadow-sm">
      <div className="text-xl font-semibold">SportsApp</div>

      <nav className="flex gap-6 text-gray-700">
        <NavLink
          to="/"
          className={({ isActive }) =>
            `transition-colors ${
              isActive ? "text-blue-600 font-medium" : "hover:text-blue-600"
            }`
          }
        >
          Home
        </NavLink>

        <NavLink
          to="/logout"
          className={({ isActive }) =>
            `transition-colors ${
              isActive ? "text-blue-600 font-medium" : "hover:text-blue-600"
            }`
          }
        >
          Logout
        </NavLink>
      </nav>
    </header>
  );
}