import { useEffect, useState } from "react";
import {
  fetchUserLeagues,
  getLeaguesBySportId,
  updateLeagueVisibility,
} from "../services/leagueService";

export default function useLeagues(sportId: number) {
  const [leagues, setLeagues] = useState<League[]>([]);
  const [loadingLeagues, setLoadingLeagues] = useState(false);
  const [userLeagues, setUserLeagues] = useState<League[]>([]);

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

  useEffect(() => {
    if (!sportId) return;

    async function loadUserLeagues() {
      setLoadingLeagues(true);
      try {
        const data = await fetchUserLeagues(sportId);
        setUserLeagues(data);
      } finally {
        setLoadingLeagues(false);
      }
    }

    loadUserLeagues();
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
    userLeagues,
    toggleLeagueVisibility,
  };
}
