export const apiBaseUrl = "http://localhost:8080";

export type AuthResponse = {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAtUtc: string;
  refreshTokenExpiresAtUtc: string;
};

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const token = localStorage.getItem("mf_token");
  const headers = new Headers(init?.headers ?? {});
  headers.set("Content-Type", "application/json");
  if (token) headers.set("Authorization", `Bearer ${token}`);

  const res = await fetch(`${apiBaseUrl}${path}`, { ...init, headers });
  if (!res.ok) {
    throw new Error(await res.text());
  }
  if (res.status === 204) return undefined as T;
  return res.json() as Promise<T>;
}

export const api = {
  login: (email: string, password: string) =>
    request<AuthResponse>("/api/auth/login", { method: "POST", body: JSON.stringify({ email, password }) }),
  refresh: (refreshToken: string) =>
    request<AuthResponse>("/api/auth/refresh", { method: "POST", body: JSON.stringify({ refreshToken }) }),
  users: {
    list: () => request<any[]>("/api/users"),
    create: (body: any) => request<any>("/api/users", { method: "POST", body: JSON.stringify(body) }),
    update: (id: string, body: any) => request<any>(`/api/users/${id}`, { method: "PUT", body: JSON.stringify(body) }),
    remove: (id: string) => request<void>(`/api/users/${id}`, { method: "DELETE" })
  },
  roles: {
    list: () => request<any[]>("/api/roles"),
    create: (body: any) => request<any>("/api/roles", { method: "POST", body: JSON.stringify(body) }),
    update: (id: string, body: any) => request<any>(`/api/roles/${id}`, { method: "PUT", body: JSON.stringify(body) }),
    assignPermissions: (id: string, permissions: string[]) =>
      request<any>(`/api/roles/${id}/permissions`, {
        method: "PUT",
        body: JSON.stringify({ permissions })
      })
  },
  permissions: {
    list: () => request<string[]>("/api/permissions")
  },
  auditLogs: {
    list: () => request<any[]>("/api/audit-logs")
  }
};
