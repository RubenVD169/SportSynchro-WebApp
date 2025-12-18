import { User } from "oidc-client-ts";

export function hasRole(
  user: User | null | undefined,
  role: string
): boolean {
  if (!user || !user.profile) return false;

  const expected = role.toLowerCase();

  const rawRoles = user.profile["roles"] ?? user.profile["role"];
  if (!rawRoles) return false;

  // multiple roles
  if (Array.isArray(rawRoles)) {
    return rawRoles
      .filter((r): r is string => typeof r === "string")
      .some(r => r.toLowerCase() === expected);
  }

  // single role
  if (typeof rawRoles === "string") {
    return rawRoles.toLowerCase() === expected;
  }

  return false;
}
