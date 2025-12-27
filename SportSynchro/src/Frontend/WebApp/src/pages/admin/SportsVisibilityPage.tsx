import { useEffect, useMemo, useState } from "react";
import { useAuth } from "react-oidc-context";
import useSports from "../../hooks/useSports";
import SportCardAdmin from "../../components/sports/SportCardAdmin";
import { FullscreenLoader } from "../../components/ui/FullscreenLoader";

export default function SportsVisibilityPage() {
  const auth = useAuth();
  const { sports, loadingSports, refreshSports } = useSports();

  const [search, setSearch] = useState("");

  useEffect(() => {
    if (auth.isLoading) return;
    if (!auth.isAuthenticated) return;

    refreshSports();
  }, [auth.isLoading, auth.isAuthenticated, refreshSports]);

  const filteredSports = useMemo(() => {
    const term = search.trim().toLowerCase();
    if (!term) return sports;

    return sports.filter((sport) =>
      sport.name.toLowerCase().includes(term)
    );
  }, [sports, search]);

  if (loadingSports) {
    return <FullscreenLoader text="Loading Sports ..." />;
  }

  return (
    <div>
      <h1 className="text-2xl text-white mb-4">Manage Sports</h1>

      <input
        type="text"
        placeholder="Search sports..."
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        className="mb-4 w-full max-w-md px-3 py-2 rounded bg-gray-700 text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-indigo-500"
      />

      {filteredSports.length === 0 ? (
        <p className="text-gray-400">No sports found.</p>
      ) : (
        <div className="grid gap-4">
          {filteredSports.map((sport) => (
            <SportCardAdmin key={sport.id} sport={sport} />
          ))}
        </div>
      )}
    </div>
  );
}
