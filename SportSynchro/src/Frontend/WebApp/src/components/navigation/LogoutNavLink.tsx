import { NavLink } from "react-router-dom";
import { useAuth } from "react-oidc-context";

export function LogoutNavLink() {
  const auth = useAuth();

  return (
    <NavLink
      to="/"
      onClick={(e) => {
        e.preventDefault();
        auth.signoutRedirect();
      }}
      className={({ isActive }) =>
        `uppercase tracking-wide transition-colors ${
          isActive
            ? "text-blue-400 font-semibold"
            : "hover:text-blue-300 text-gray-300"
        }`
      }
    >
      Logout
    </NavLink>
  );
}
