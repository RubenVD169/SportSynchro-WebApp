import { api } from "../lib/api";

interface CreateCheckoutResponse {
  url: string;
  sessionId: string;
}

export async function createCheckoutSession(): Promise<CreateCheckoutResponse> {
  const res = await api.post<CreateCheckoutResponse>("/stripe/checkout");

  if (!res.data?.url) {
    throw new Error("Stripe checkout URL missing in response");
  }

  return res.data;
}
