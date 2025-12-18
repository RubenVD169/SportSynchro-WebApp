interface LeagueCardProps {
    league: {
        id: number;
        name: string;
    };
}

export default function LeagueCard({ league }: LeagueCardProps) {
    return (
        <div className="p-3 bg-gray-800 rounded-lg shadow text-white">
            <h3 className="text-md font-semibold">{league.name}</h3>
        </div>
    );
}
