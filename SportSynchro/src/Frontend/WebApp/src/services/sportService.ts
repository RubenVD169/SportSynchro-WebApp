import { api } from "../lib/api";

export async function fetchSports() {
  const res = await api.get("/sports");
  return res.data;
}

export async function updateSportVisibility(
  sportId: number,
  visible: boolean
) {
  const res = await api.patch(`/sports/${sportId}/visibility`, {
    visible,
  });

  return res.data;
}
