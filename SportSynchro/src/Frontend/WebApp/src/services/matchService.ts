import { api } from "../lib/api";
import axios from "axios";

export async function fetchRecentMatchesByLeague(
  leagueId: number
): Promise<Match[]> {
  try {
    const res = await api.get(`/match/${leagueId}`);
    return res.data;
  } catch (error) {
    if (axios.isAxiosError(error) && error.response?.status === 404) {
      return [];
    } else {
      throw error;
    }
  }
}

export async function fetchScheduledMatchesByLeague(
  leagueId: number
): Promise<Match[]> {
  try {
    const res = await api.get(`/match/schedule/${leagueId}`);
    return res.data;
  } catch (error) {
    if (axios.isAxiosError(error) && error.response?.status === 404) {
      return [];
    } else {
      throw error;
    }
  }
}

export async function fetchLiveMatches(leagueId: number): Promise<LiveMatch[]> {
  try {
    const res = await api.get(`livescores/league/${leagueId}`);
    return res.data;
  } catch (error) {
    if (axios.isAxiosError(error) && error.response?.status === 404) {
      return [];
    } else {
      throw error;
    }
  }
}

type DerivedMatchStatus = "not-started" | "live" | "finished";

export function deriveMatchStatus(
  backendStatus: "Not Started" | "Finished",
  matchDateIso: string
): DerivedMatchStatus {
  if (backendStatus === "Finished") {
    return "finished";
  }

  const start = new Date(matchDateIso);
  const now = new Date();

  const LIVE_MAX_MS = 3 * 60 * 60 * 1000; // 3 hours so when something goes wrong we don't have matches live forever
  const liveEnd = new Date(start.getTime() + LIVE_MAX_MS);

  if (now < start) {
    return "not-started";
  }

  if (now >= start && now <= liveEnd) {
    return "live";
  }

  return "finished";
}
