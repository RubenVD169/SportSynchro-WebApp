import { useEffect, useState } from "react";
import {
  getLeaguesBySportId,
  updateLeagueVisibility,
} from "../services/leagueService";

export function useAdminLeagues(sportId: number) {
  const [leagues, setLeagues] = useState<League[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!sportId) return;

    async function load() {
      setLoading(true);
      try {
        const data = await getLeaguesBySportId(sportId);
        setLeagues(data);
      } finally {
        setLoading(false);
      }
    }

    load();
  }, [sportId]);

  async function toggleVisibility(id: number, current: boolean) {
    const newValue = !current;

    await updateLeagueVisibility(id, newValue);

    setLeagues(prev =>
      prev.map(l =>
        l.id === id ? { ...l, visible: newValue } : l
      )
    );
  }

  return {
    leagues,
    loading,
    toggleVisibility,
  };
}
