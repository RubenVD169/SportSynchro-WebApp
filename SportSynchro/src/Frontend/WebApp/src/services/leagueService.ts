import { api } from "../lib/api";

export async function getLeaguesBySportId(sportId: number): Promise<League[]> {
  const res = await api.get(`/admin/leagues/${sportId}`);
  return res.data;
}

export async function updateLeagueVisibility(
  leagueId: number,
  visible: boolean
): Promise<League> {
  const res = await api.patch(`/admin/leagues/${leagueId}/visibility`, {
    visible,
  });
  return res.data;
}

export async function fetchUserLeagues(sportId: number): Promise<League[]> {
  const res = await api.get(`/user/leagues/${sportId}`);
  return res.data;
}


