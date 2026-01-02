import { api } from "../lib/api";

export async function getHasLiveAccess(): Promise<boolean> {
  const res = await api.get<boolean>("/subscription/me");
  return res.data;
}
