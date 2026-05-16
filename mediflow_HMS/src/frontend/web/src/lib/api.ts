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

export const workflowApi = {
  patients: {
    create: (body: any) => request<any>("/api/patients", { method: "POST", body: JSON.stringify(body) }),
    search: (params: URLSearchParams) => request<any[]>(`/api/patients/search?${params.toString()}`)
  },
  appointments: {
    create: (body: any) => request<any>("/api/appointments", { method: "POST", body: JSON.stringify(body) }),
    daily: (date: string) => request<any[]>(`/api/appointments/daily?date=${date}`)
  },
  queue: {
    list: (date: string) => request<any[]>(`/api/queue?date=${date}`),
    updateState: (id: string, state: string) => request<any>(`/api/queue/${id}/state`, { method: "PUT", body: JSON.stringify({ state }) })
  }
};

export const consultationApi = {
  start: (body: any) => request<any>("/api/consultations/start", { method: "POST", body: JSON.stringify(body) }),
  save: (encounterId: string, body: any) => request<any>(`/api/consultations/${encounterId}`, { method: "PUT", body: JSON.stringify(body) }),
  waiting: (date: string) => request<any[]>(`/api/consultations/waiting?date=${date}`),
  history: (patientId: string) => request<any[]>(`/api/consultations/history/${patientId}`)
};

export const admissionApi = {
  config: {
    hierarchy: () => request<any>("/api/admission/config/hierarchy"),
    createBuilding: (body: any) => request<any>("/api/admission/config/buildings", { method: "POST", body: JSON.stringify(body) }),
    createFloor: (body: any) => request<any>("/api/admission/config/floors", { method: "POST", body: JSON.stringify(body) }),
    createWard: (body: any) => request<any>("/api/admission/config/wards", { method: "POST", body: JSON.stringify(body) }),
    createRoom: (body: any) => request<any>("/api/admission/config/rooms", { method: "POST", body: JSON.stringify(body) }),
    createBed: (body: any) => request<any>("/api/admission/config/beds", { method: "POST", body: JSON.stringify(body) })
  },
  admissions: {
    create: (body: any) => request<any>("/api/admissions", { method: "POST", body: JSON.stringify(body) }),
    assignBed: (id: string, bedId: string) => request<any>(`/api/admissions/${id}/assign-bed`, { method: "PUT", body: JSON.stringify({ bedId }) }),
    transferBed: (id: string, toBedId: string, reason: string) => request<any>(`/api/admissions/${id}/transfer-bed`, { method: "PUT", body: JSON.stringify({ toBedId, reason }) }),
    list: () => request<any[]>("/api/admissions"),
    bedBoard: () => request<any[]>("/api/admissions/bed-board"),
    floorReception: () => request<any>("/api/admissions/floor-reception")
  }
};
