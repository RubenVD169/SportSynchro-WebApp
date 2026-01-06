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
  leagueName: string;
  homeTeam: string;
  awayTeam: string;
  homeScore: number;
  awayScore: number;
  matchDate: string; // ISO date string
  status: "Not Started" | "Finished";
}

interface LiveMatch {
  id: string;
  homeTeam: string;
  awayTeam: string;
  homeScore: number;
  awayScore: number;
  status?: string;
  progress?: string;
}
