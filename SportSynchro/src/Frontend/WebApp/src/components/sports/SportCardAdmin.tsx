import useSports from "../../hooks/useSports";

interface SportCardAdminProps {
  sport: {
    id: number;
    name: string;
    visible: boolean;
  };
}

export default function SportCardAdmin({ sport }: SportCardAdminProps) {
  const { toggleSportVisibility, selectSport } = useSports();

  return (
    <div className="p-4 bg-gray-800 rounded-lg shadow flex justify-between items-center">
      <div>
        <h2 className="text-lg font-semibold text-white">{sport.name}</h2>
      </div>

      <div className="flex items-center gap-4">
        <button
          onClick={() => toggleSportVisibility(sport.id, sport.visible)}
          className="px-3 py-1 bg-blue-600 rounded text-white"
        >
          {sport.visible ? "Hide" : "Show"}
        </button>

        <button
          onClick={() => selectSport(sport.id)}
          className="px-3 py-1 bg-green-600 rounded text-white"
        >
          Leagues
        </button>
      </div>
    </div>
  );
}
