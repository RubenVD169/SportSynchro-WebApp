import { useEffect, useState } from "react";
import { fetchUserLeagues } from "../services/leagueService";

export function useUserLeagues(sportId: number) {
  const [leagues, setLeagues] = useState<League[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!sportId) return;

    async function load() {
      setLoading(true);
      try {
        const data = await fetchUserLeagues(sportId);
        setLeagues(data);
      } finally {
        setLoading(false);
      }
    }

    load();
  }, [sportId]);

  return {
    leagues,
    loading,
  };
}
