import { useEffect, useState } from "react";
import { fetchRecentMatchesByLeague } from "../services/matchService";

export default function useRecentMatches(leagueId: number) {
  const [matches, setMatches] = useState<Match[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!leagueId) return;

    async function load() {
      setLoading(true);
      try {
        const data = await fetchRecentMatchesByLeague(leagueId);
        setMatches(data);
      } finally {
        setLoading(false);
      }
    }

    load();
  }, [leagueId]);

  return {
    matches,
    loading,
  };
}
