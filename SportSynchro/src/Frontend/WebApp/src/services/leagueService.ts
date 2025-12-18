import axios from "axios";

const API_BASE = "/api"; //TODO: move to env variable + config

export async function getLeaguesBySport(sportId: number): Promise<League[]> {
  const res = await axios.get(`${API_BASE}/sports/${sportId}/leagues`);
  return res.data;
}

export async function updateLeagueVisibility(
  leagueId: number,
  visible: boolean
): Promise<League> {
  const res = await axios.patch(`${API_BASE}/leagues/${leagueId}/visibility`, {
    visible,
  });
  return res.data;
}
