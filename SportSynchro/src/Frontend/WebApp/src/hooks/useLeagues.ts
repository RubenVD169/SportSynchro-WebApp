import { useEffect, useState } from "react";
import {
  getLeaguesBySportId,
  updateLeagueVisibility,
} from "../services/leagueService";

export default function useLeagues(sportId: number) {
  const [leagues, setLeagues] = useState<League[]>([]);
  const [loadingLeagues, setLoadingLeagues] = useState(false);

  useEffect(() => {
    if (!sportId) return;

    async function load() {
      setLoadingLeagues(true);
      try {
        const data = await getLeaguesBySportId(sportId);
        setLeagues(data);
      } finally {
        setLoadingLeagues(false);
      }
    }

    load();
  }, [sportId]);

  async function toggleLeagueVisibility(id: number, current: boolean) {
    const newValue = !current;

    await updateLeagueVisibility(id, newValue);

    setLeagues((prev) =>
      prev.map((league) =>
        league.id === id ? { ...league, visible: newValue } : league
      )
    );
  }

  return {
    leagues,
    loadingLeagues,
    toggleLeagueVisibility,
  };
}
