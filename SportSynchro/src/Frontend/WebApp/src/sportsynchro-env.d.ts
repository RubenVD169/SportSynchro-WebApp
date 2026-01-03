interface Sport {
  id: number;
  name: string;
  visible: boolean;
}

interface League {
  id: number;
  sportId: number;
  name: string;
  visible: boolean;
}

interface Match {
  id: number;
  leagueId: number;
  homeTeam: string;
  awayTeam: string;
  homeScore: number;
  awayScore: number;
  matchDate: string; // ISO date string
  status: "Not Started" | "Finished";
}
