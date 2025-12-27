import { useNavigate } from "react-router-dom";
import useSports from "../../hooks/useSports";

interface SportCardAdminProps {
  sport: {
    id: number;
    name: string;
    visible: boolean;
  };
}

export default function SportCardAdmin({ sport }: SportCardAdminProps) {
  const { toggleSportVisibility } = useSports();
  const navigate = useNavigate();

  const visibilityClasses = sport.visible
    ? "bg-emerald-600 hover:bg-emerald-700 focus:ring-emerald-500"
    : "bg-rose-600 hover:bg-rose-700 focus:ring-rose-500";

  return (
    <div className="p-4 bg-gray-800 rounded-lg shadow flex justify-between items-center">
      <h2 className="text-lg font-semibold text-white">{sport.name}</h2>

      <div className="flex items-center gap-3">
        <button
          onClick={() => toggleSportVisibility(sport.id, sport.visible)}
          className={`px-4 py-1.5 rounded text-white font-medium transition focus:outline-none focus:ring-2 ${visibilityClasses} cursor-pointer`}
        >
          {sport.visible ? "Shown" : "Hidden"}
        </button>

        <button
          onClick={() => navigate(`/admin/sports/${sport.id}/leagues`)}
          className="px-4 py-1.5 rounded font-medium text-white bg-indigo-600 hover:bg-indigo-700 transition focus:outline-none focus:ring-2 focus:ring-indigo-500 cursor-pointer"
        >
          Leagues
        </button>
      </div>
    </div>
  );
}
