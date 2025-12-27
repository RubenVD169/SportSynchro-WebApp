import { useMemo, useState } from "react";
import { useParams } from "react-router-dom";
import useLeagues from "../../hooks/useLeagues";
import LeagueCardAdmin from "../../components/sports/LeagueCardAdmin";

export default function LeagueVisibilityPage() {
  const { sportId } = useParams<{ sportId: string }>();
  const { leagues, loadingLeagues, toggleLeagueVisibility } = useLeagues(
    Number(sportId)
  );

  const [search, setSearch] = useState("");

  const filteredLeagues = useMemo(() => {
    const term = search.trim().toLowerCase();
    if (!term) return leagues;

    return leagues.filter((league) =>
      league.name.toLowerCase().includes(term)
    );
  }, [leagues, search]);

  if (loadingLeagues) {
    return <p className="text-gray-300">Loading leagues...</p>;
  }

  return (
    <div>
      <h1 className="text-2xl text-white mb-4">Manage Leagues</h1>

      <input
        type="text"
        placeholder="Search leagues..."
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        className="mb-4 w-full max-w-md px-3 py-2 rounded bg-gray-700 text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-indigo-500"
      />

      {filteredLeagues.length === 0 ? (
        <p className="text-gray-400">No leagues found.</p>
      ) : (
        <div className="space-y-3">
          {filteredLeagues.map((league) => (
            <LeagueCardAdmin
              key={league.id}
              league={league}
              onToggleVisibility={toggleLeagueVisibility}
            />
          ))}
        </div>
      )}
    </div>
  );
}
