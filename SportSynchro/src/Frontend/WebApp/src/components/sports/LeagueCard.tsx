import useRecentMatches from "../../hooks/useRecentMatches";
import { useState } from "react";
import { createCheckoutSession } from "../../services/stripeService";
import { useNavigate, useParams } from "react-router-dom";

interface LeagueCardProps {
  league: League;
  hasLiveAccess: boolean;
}

export default function LeagueCard({
  league,
  hasLiveAccess,
}: LeagueCardProps) {
  const navigate = useNavigate();
  const { matches, loading } = useRecentMatches(league.id);
  const [redirecting, setRedirecting] = useState(false);
  const sportId = useParams().id;

  async function handleUnlockClick() {
    try {
      setRedirecting(true);

      const { url } = await createCheckoutSession();

      window.location.href = url;
    } catch (err) {
      console.error("Failed to start Stripe checkout", err);
      setRedirecting(false);
    }
  }

  function handleCardClick() {
    navigate(`/sports/${sportId}/${league.id}`, { 
      state: { leagueName: league.name } 
    });
  }

  return (
    <div className="bg-gray-800 rounded-lg p-4 shadow flex flex-col gap-4 cursor-pointer" 
      onClick={hasLiveAccess ? handleCardClick : handleUnlockClick}>
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
        <button
          onClick={handleUnlockClick}
          disabled={redirecting}
          className="mt-auto bg-indigo-600 hover:bg-indigo-700 disabled:bg-indigo-400 text-white py-2 rounded font-medium cursor-pointer"
        >
          {redirecting ? "Redirecting..." : "Unlock full live access"}
        </button>
      )}
    </div>
  );
}
