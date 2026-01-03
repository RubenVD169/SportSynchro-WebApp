import { useEffect, useState } from "react";
import { fetchLiveMatches } from "../services/matchService";

const POLL_INTERVAL_MS = 120_000; // 120s

export default function useLiveMatches(leagueId: number) {
  const [liveMatches, setLiveMatches] = useState<LiveMatch[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const intervalId = window.setInterval(load, POLL_INTERVAL_MS);

    async function load() {
      setLoading(true);
      try {
        const data = await fetchLiveMatches(leagueId);
        setLiveMatches(data);
      } finally {
        setLoading(false);
      }
    }

    load();

    return () => {
      window.clearInterval(intervalId);
    };
  }, [leagueId]);

  return {
    liveMatches,
    loading,
  };
}
