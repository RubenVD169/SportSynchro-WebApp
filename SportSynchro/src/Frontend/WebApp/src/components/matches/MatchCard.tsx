import { deriveMatchStatus } from "../../services/matchService";

interface MatchCardProps {
  match: {
    id: number;
    homeTeam: string;
    awayTeam: string;
    homeScore: number;
    awayScore: number;
    matchDate: string; // ISO
    status: "Not Started" | "Finished"; 
  };
}

export default function MatchCard({ match }: MatchCardProps) {
  const d = new Date(match.matchDate);
  const derivedStatus = deriveMatchStatus(match.status, match.matchDate);


  const date = d.toLocaleDateString("nl-BE", {
    weekday: "short",
    day: "2-digit",
    month: "short",
  });

  const time = d.toLocaleTimeString("nl-BE", {
    hour: "2-digit",
    minute: "2-digit",
  });

    const cardStyle =
    derivedStatus === "finished"
      ? "bg-gray-900/70 border-gray-700"
      : derivedStatus === "live"
      ? "bg-gradient-to-br from-gray-900 via-gray-900 to-red-900/30 border-red-400/60 shadow-[0_0_18px_rgba(239,68,68,0.4)]"
      : "bg-gray-800/40 border-gray-600";

  return (
    <div
      className={`
        ${cardStyle}
        rounded-xl
        p-4
        hover:border-indigo-500/40
        transition
      `}
    >
      <div className="flex items-center justify-between mb-3">
        <div className="text-xs text-gray-400">
          {date} · {time}
        </div>
        {derivedStatus === "finished" && (
          <div className="text-sm font-semibold text-white">
            {match.homeScore} – {match.awayScore}
          </div>
        )}

        {derivedStatus === "live" && (
          <span className="text-xs font-semibold text-red-300 animate-pulse">
            LIVE
          </span>
        )}

        {derivedStatus === "not-started" && (
          <span className="text-xs font-medium text-indigo-400">
            Scheduled
          </span>
        )}

      </div>

      <div className="space-y-2 text-sm">
        <div className="flex items-center justify-between">
          <span className="text-gray-200 font-medium">
            {match.homeTeam}
          </span>
          <span className="text-gray-500 text-xs">Home</span>
        </div>

        <div className="flex items-center justify-between">
          <span className="text-gray-200 font-medium">
            {match.awayTeam}
          </span>
          <span className="text-gray-500 text-xs">Away</span>
        </div>
      </div>
    </div>
  );
}
