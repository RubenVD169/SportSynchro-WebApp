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
    }else {
      throw error;
    }
  }
}
