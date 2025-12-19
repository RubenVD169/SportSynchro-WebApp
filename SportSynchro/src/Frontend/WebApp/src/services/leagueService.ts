import axios from "axios";

const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;

export async function getLeaguesBySport(sportId: number): Promise<League[]> {
  const res = await axios.get(`${apiBaseUrl}/sports/${sportId}/leagues`);
  return res.data;
}

export async function updateLeagueVisibility(
  leagueId: number,
  visible: boolean
): Promise<League> {
  const res = await axios.patch(`${apiBaseUrl}/leagues/${leagueId}/visibility`, {
    visible,
  });
  return res.data;
}
