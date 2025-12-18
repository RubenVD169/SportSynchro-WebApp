import { useEffect, useState } from "react";
import { getLeaguesBySport } from "../services/leagueService";

export default function useLeague(sportId: number | null) {
  const [leagues, setLeagues] = useState<League[]>([]);
  const [loadingLeagues, setLoadingLeagues] = useState(false);

  useEffect(() => {
    if (!sportId) return;

     
    // eslint-disable-next-line react-hooks/set-state-in-effect
    setLoadingLeagues(true);

    getLeaguesBySport(sportId).then((data) => {
      setLeagues(data);
      setLoadingLeagues(false);
    });
  }, [sportId]);

  return { leagues, loadingLeagues };
}
