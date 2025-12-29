interface LeagueCardAdminProps {
  league: {
    id: number;
    name: string;
    visible: boolean;
  };
  onToggleVisibility: (id: number, current: boolean) => void;
}

export default function LeagueCardAdmin({
  league,
  onToggleVisibility,
}: LeagueCardAdminProps) {
  const visibilityClasses = league.visible
    ? "bg-emerald-600 hover:bg-emerald-700"
    : "bg-rose-600 hover:bg-rose-700";

  return (
    <div className="p-4 bg-gray-800 rounded-lg shadow flex justify-between items-center">
      <h2 className="text-lg font-semibold text-white">{league.name}</h2>

      <button
        onClick={() => onToggleVisibility(league.id, league.visible)}
        className={`px-4 py-1.5 rounded text-white font-medium transition cursor-pointer ${visibilityClasses}`}
      >
        {league.visible ? "Shown" : "Hidden"}
      </button>
    </div>
  );
}
