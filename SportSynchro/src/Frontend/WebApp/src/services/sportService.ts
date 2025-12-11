import axios from "axios";

const API_BASE = "/api"; //TODO: move to env variable + config

export async function fetchSports() {
  const res = await axios.get(`${API_BASE}/sports`);
  return res.data;
}

export async function updateSportVisibility(sportId: number, visible: boolean) {
  const res = await axios.patch(`${API_BASE}/sports/${sportId}/visibility`, {
    visible,
  });
  return res.data;
}
