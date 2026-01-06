import { useEffect, useState } from "react";
import { fetchRecentMatchesBySport } from "../services/matchService";

export default function useRecentMatchesBySport(sportId: number) {
  const [matches, setMatches] = useState<Match[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!sportId) return;

    async function load() {
      setLoading(true);
      try {
        const data = await fetchRecentMatchesBySport(sportId);
        setMatches(data);
      } finally {
        setLoading(false);
      }
    }

    load();
  }, [sportId]);

  return {
    matches,
    loading,
  };
}
