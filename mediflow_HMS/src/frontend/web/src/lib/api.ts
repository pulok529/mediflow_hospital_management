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

export const nursingApi = {
  dashboard: () => request<any>("/api/nursing/dashboard"),
  notes: {
    add: (body: any) => request<any>("/api/nursing/notes", { method: "POST", body: JSON.stringify(body) }),
    list: (admissionId: string) => request<any[]>(`/api/nursing/notes/${admissionId}`)
  },
  medications: {
    add: (body: any) => request<any>("/api/nursing/medications", { method: "POST", body: JSON.stringify(body) }),
    due: () => request<any[]>("/api/nursing/medications/due"),
    administer: (id: string, remarks?: string) => request<any>(`/api/nursing/medications/${id}/administer`, { method: "PUT", body: JSON.stringify({ remarks }) })
  },
  vitals: {
    schedule: (body: any) => request<any>("/api/nursing/vitals/schedule", { method: "POST", body: JSON.stringify(body) }),
    record: (id: string, body: any) => request<any>(`/api/nursing/vitals/${id}/record`, { method: "PUT", body: JSON.stringify(body) })
  },
  handover: {
    add: (body: any) => request<any>("/api/nursing/handover", { method: "POST", body: JSON.stringify(body) })
  },
  transferDischarge: () => request<any[]>("/api/nursing/transfer-discharge")
};

export const diagnosticsApi = {
  lab: {
    catalog: () => request<any[]>("/api/diagnostics/lab/catalog"),
    addCatalog: (body: any) => request<any>("/api/diagnostics/lab/catalog", { method: "POST", body: JSON.stringify(body) }),
    orders: () => request<any[]>("/api/diagnostics/lab/orders"),
    createOrder: (body: any) => request<any>("/api/diagnostics/lab/orders", { method: "POST", body: JSON.stringify(body) }),
    collect: (id: string) => request<any>(`/api/diagnostics/lab/orders/${id}/collect`, { method: "PUT" }),
    result: (id: string, body: any) => request<any>(`/api/diagnostics/lab/orders/${id}/result`, { method: "PUT", body: JSON.stringify(body) }),
    approve: (id: string) => request<any>(`/api/diagnostics/lab/orders/${id}/approve`, { method: "PUT" })
  },
  imaging: {
    catalog: () => request<any[]>("/api/diagnostics/imaging/catalog"),
    addCatalog: (body: any) => request<any>("/api/diagnostics/imaging/catalog", { method: "POST", body: JSON.stringify(body) }),
    orders: () => request<any[]>("/api/diagnostics/imaging/orders"),
    createOrder: (body: any) => request<any>("/api/diagnostics/imaging/orders", { method: "POST", body: JSON.stringify(body) }),
    schedule: (id: string, body: any) => request<any>(`/api/diagnostics/imaging/orders/${id}/schedule`, { method: "PUT", body: JSON.stringify(body) }),
    report: (id: string, body: any) => request<any>(`/api/diagnostics/imaging/orders/${id}/report`, { method: "PUT", body: JSON.stringify(body) }),
    approve: (id: string) => request<any>(`/api/diagnostics/imaging/orders/${id}/approve`, { method: "PUT" })
  }
};

export const pharmacyApi = {
  dashboard: () => request<any>("/api/pharmacy/dashboard"),
  medicines: () => request<any[]>("/api/pharmacy/medicines"),
  addMedicine: (body: any) => request<any>("/api/pharmacy/medicines", { method: "POST", body: JSON.stringify(body) }),
  batches: () => request<any[]>("/api/pharmacy/batches"),
  addBatch: (body: any) => request<any>("/api/pharmacy/batches", { method: "POST", body: JSON.stringify(body) }),
  opdDispense: (body: any) => request<any>("/api/pharmacy/dispense/opd", { method: "POST", body: JSON.stringify(body) }),
  ipdIssue: (body: any) => request<any>("/api/pharmacy/issue/ipd", { method: "POST", body: JSON.stringify(body) }),
  returns: (body: any) => request<any>("/api/pharmacy/returns", { method: "POST", body: JSON.stringify(body) }),
  lowStock: () => request<any[]>("/api/pharmacy/low-stock-alerts"),
  suppliers: () => request<any[]>("/api/pharmacy/suppliers"),
  addSupplier: (body: any) => request<any>("/api/pharmacy/suppliers", { method: "POST", body: JSON.stringify(body) }),
  createPr: (body: any) => request<any>("/api/pharmacy/purchase-requests", { method: "POST", body: JSON.stringify(body) }),
  prs: () => request<any[]>("/api/pharmacy/purchase-requests"),
  createPo: (body: any) => request<any>("/api/pharmacy/purchase-orders", { method: "POST", body: JSON.stringify(body) }),
  pos: () => request<any[]>("/api/pharmacy/purchase-orders"),
  receive: (body: any) => request<any>("/api/pharmacy/goods-receive", { method: "POST", body: JSON.stringify(body) }),
  grns: () => request<any[]>("/api/pharmacy/goods-receive"),
  issueDept: (body: any) => request<any>("/api/pharmacy/issue/department", { method: "POST", body: JSON.stringify(body) }),
  movements: () => request<any[]>("/api/pharmacy/stock-movements")
};

export const billingApi = {
  dashboard: () => request<any>("/api/billing/dashboard"),
  bills: () => request<any[]>("/api/billing/bills"),
  bill: (id: string) => request<any>(`/api/billing/bills/${id}`),
  createOpd: (body: any) => request<any>("/api/billing/opd", { method: "POST", body: JSON.stringify(body) }),
  createIpd: (body: any) => request<any>("/api/billing/ipd", { method: "POST", body: JSON.stringify(body) }),
  addRunningLine: (body: any) => request<any>("/api/billing/running-line", { method: "PUT", body: JSON.stringify(body) }),
  addPayment: (body: any) => request<any>("/api/billing/payments", { method: "POST", body: JSON.stringify(body) }),
  requestDiscount: (body: any) => request<any>("/api/billing/discount/request", { method: "POST", body: JSON.stringify(body) }),
  discountQueue: () => request<any[]>("/api/billing/discount-queue"),
  resolveDiscount: (id: string, status: string) => request<any>(`/api/billing/discount/${id}/resolve`, { method: "PUT", body: JSON.stringify({ status }) }),
  refund: (body: any) => request<any>("/api/billing/refunds", { method: "POST", body: JSON.stringify(body) }),
  finalClearance: (body: any) => request<any>("/api/billing/final-clearance", { method: "POST", body: JSON.stringify(body) }),
  saveDischarge: (body: any) => request<any>("/api/billing/discharge-summary", { method: "POST", body: JSON.stringify(body) })
};
