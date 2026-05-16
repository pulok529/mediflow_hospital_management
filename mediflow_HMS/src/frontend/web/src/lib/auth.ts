export type DecodedToken = {
  sub?: string;
  email?: string;
  role?: string | string[];
  permission?: string | string[];
  exp?: number;
};

function decodeBase64Url(input: string) {
  const base64 = input.replace(/-/g, "+").replace(/_/g, "/");
  const padded = base64.padEnd(base64.length + ((4 - (base64.length % 4)) % 4), "=");
  return atob(padded);
}

export function decodeToken(token: string | null): DecodedToken | null {
  if (!token) return null;
  try {
    const [, payload] = token.split(".");
    if (!payload) return null;
    return JSON.parse(decodeBase64Url(payload));
  } catch {
    return null;
  }
}

export function getRoles(token: string | null): string[] {
  const decoded = decodeToken(token);
  if (!decoded?.role) return [];
  return Array.isArray(decoded.role) ? decoded.role : [decoded.role];
}

export function getPermissions(token: string | null): string[] {
  const decoded = decodeToken(token);
  if (!decoded?.permission) return [];
  return Array.isArray(decoded.permission) ? decoded.permission : [decoded.permission];
}

export function isTokenExpired(token: string | null): boolean {
  const decoded = decodeToken(token);
  if (!decoded?.exp) return true;
  return Date.now() >= decoded.exp * 1000;
}
