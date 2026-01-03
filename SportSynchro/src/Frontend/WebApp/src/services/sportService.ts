import { api } from "../lib/api";

export async function fetchAdminSports() {
  const res = await api.get("admin/sports");
  return res.data;
}

export async function updateSportVisibility(
  sportId: number,
  visible: boolean
) {
  const res = await api.patch(`admin/sports/${sportId}/visibility`, {
    visible,
  });

  return res.data;
}

export async function fetchUserSports() {
  const res = await api.get("user/sports");
  return res.data;
}