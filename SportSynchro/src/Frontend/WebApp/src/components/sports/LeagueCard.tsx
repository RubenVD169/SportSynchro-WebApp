import useRecentMatches from "../../hooks/useRecentMatches";

interface LeagueCardProps {
  league: League;
  hasLiveAccess: boolean;
}

export default function LeagueCard({
  league,
  hasLiveAccess,
}: LeagueCardProps) {
  const { matches, loading } = useRecentMatches(league.id);

  return (
    <div className="bg-gray-800 rounded-lg p-4 shadow flex flex-col gap-4">
      <h2 className="text-xl font-semibold text-white">{league.name}</h2>

      <div>
        <h3 className="text-sm font-medium text-gray-300 mb-2">
          Recent matches
        </h3>

        {loading ? (
          <p className="text-sm text-gray-500">Loading matches...</p>
        ) : matches.length > 0 ? (
          <ul className="text-sm text-gray-400 space-y-1">
            {matches.slice(0, 3).map((m) => (
              <li key={m.id}>
                {m.homeTeam} {m.homeScore} – {m.awayScore} {m.awayTeam}
              </li>
            ))}
          </ul>
        ) : (
          <p className="text-sm text-gray-500">
            No recent matches available.
          </p>
        )}
      </div>

      <div className="relative bg-gray-700 rounded p-3">
        {!hasLiveAccess && (
          <div className="absolute inset-0 bg-black/60 flex items-center justify-center rounded">
            <span className="text-white font-medium">
              Live matches locked
            </span>
          </div>
        )}

        <h3 className="text-sm font-medium text-gray-300">
          Live matches
        </h3>
        <p className="text-xs text-gray-400">
          Real-time scores & statistics
        </p>
      </div>

      {!hasLiveAccess && (
        <button className="mt-auto bg-indigo-600 hover:bg-indigo-700 text-white py-2 rounded font-medium cursor-pointer">
          Unlock full live access
        </button>
      )}
    </div>
  );
}
