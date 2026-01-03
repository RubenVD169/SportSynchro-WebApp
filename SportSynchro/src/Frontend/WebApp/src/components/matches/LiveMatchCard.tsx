interface LiveMatchCardProps {
    match: LiveMatch;
}

export default function LiveMatchCard({ match }: LiveMatchCardProps) {

    const timeLabel = match.status === "HT"
        ? "Halftime"
        : "Live";

    const cardStyle =
        "bg-gradient-to-br from-gray-900 via-gray-900 to-red-900/30 " +
        "border border-red-400/60 shadow-[0_0_18px_rgba(239,68,68,0.4)]";

    return (
        <div
            className={`${cardStyle} rounded-xl p-4 hover:border-red-300/70 transition`}
        >
            <div className="flex items-center justify-between mb-3">
                <div className="text-xs text-gray-400">
                    {timeLabel}
                    {match.progress && (
                        <span className="ml-2 text-red-300 font-medium">
                            · {match.progress}
                        </span>
                    )}
                </div>

                <div className="text-sm font-semibold text-white">
                    {match.homeScore} – {match.awayScore}
                </div>

                <span className="text-xs font-semibold text-red-300 animate-pulse">
                    LIVE
                </span>
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