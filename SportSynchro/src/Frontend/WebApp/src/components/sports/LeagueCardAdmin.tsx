interface LeagueCardAdminProps {
    league: {
        id: number;
        name: string;
        visible: boolean;
    };
    onToggleVisibility: (leagueId: number, currentVisible: boolean) => void;
}

export default function LeagueCardAdmin({ league, onToggleVisibility }: LeagueCardAdminProps) {
    return (
        <div className="p-4 bg-gray-800 rounded-lg shadow flex justify-between items-center">
            <h2 className="text-lg font-semibold text-white">{league.name}</h2>

            <button
                onClick={() => onToggleVisibility(league.id, league.visible)}
                className="px-3 py-1 bg-blue-600 rounded text-white"
            >
                {league.visible ? "Hide" : "Show"}
            </button>
        </div>
    );
}
