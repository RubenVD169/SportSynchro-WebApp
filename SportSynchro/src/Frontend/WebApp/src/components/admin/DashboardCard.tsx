import { Link } from "react-router-dom";

export default function DashboardCard({ to, title, description }) {
  return (
    <Link
      to={to}
      className="bg-gray-850 p-6 rounded-lg shadow hover:shadow-lg transition-all border border-gray-800 hover:border-blue-400"
    >
      <h2 className="text-xl font-semibold text-blue-300 mb-2">{title}</h2>
      <p className="text-gray-400 text-sm">{description}</p>
    </Link>
  );
}
