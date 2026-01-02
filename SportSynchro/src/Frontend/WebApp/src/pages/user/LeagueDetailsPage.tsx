import { useParams } from "react-router-dom";
import { useState } from "react";
import useRecentMatches from "../../hooks/useRecentMatches";
// later: useLiveMatches

export default function LeagueDetailsPage() {
    const { leagueId } = useParams<{
        leagueId: string;
    }>();

    const [activeTab, setActiveTab] = useState<"recent" | "live">("recent");

    const { matches, loading } = useRecentMatches(Number(leagueId));

    return (
        <div className="p-6 max-w-5xl mx-auto">
            <div className="mb-6">
                <h1 className="text-2xl font-semibold text-white">
                    League details
                </h1>
                <p className="text-sm text-gray-400">
                    Matches & live results
                </p>
            </div>
            <div className="flex gap-2 mb-6">
                <button
                    onClick={() => setActiveTab("recent")}
                    className={`px-4 py-2 rounded text-sm font-medium
                    ${activeTab === "recent"
                            ? "bg-indigo-600 text-white"
                            : "bg-gray-800 text-gray-400 hover:text-white"
                        }`}
                >
                    Schedule
                </button>

                <button
                    onClick={() => setActiveTab("live")}
                    className={`px-4 py-2 rounded text-sm font-medium
                    ${activeTab === "live"
                            ? "bg-indigo-600 text-white"
                            : "bg-gray-800 text-gray-400 hover:text-white"
                        }`}
                >
                    Live matches
                </button>
            </div>
            {activeTab === "recent" && (
                <div className="bg-gray-800 rounded-lg p-4">
                    {loading ? (
                        <p className="text-sm text-gray-500">Loading matches…</p>
                    ) : matches.length === 0 ? (
                        <p className="text-sm text-gray-500">
                            No recent matches available.
                        </p>
                    ) : (
                        <ul className="divide-y divide-gray-700">
                            {matches.map((m) => (
                                <li
                                    key={m.id}
                                    className="py-3 flex items-center justify-between"
                                >
                                    <div className="text-sm text-gray-300">
                                        {m.homeTeam} vs {m.awayTeam}
                                    </div>

                                    <div className="text-sm font-medium text-white">
                                        {m.homeScore} – {m.awayScore}
                                    </div>
                                </li>
                            ))}
                        </ul>
                    )}
                </div>
            )}
            {activeTab === "live" && (
                <div className="bg-gray-800 rounded-lg p-4">
                    <div className="flex items-center justify-center h-32">
                        <p className="text-sm text-gray-500">
                            No live matches at the moment.
                        </p>
                    </div>
                </div>
            )}
        </div>
    );
}
