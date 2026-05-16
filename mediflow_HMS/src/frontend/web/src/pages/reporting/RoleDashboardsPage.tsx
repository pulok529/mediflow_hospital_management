import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { reportingApi } from "@/lib/api";

export function RoleDashboardsPage() {
  const [role, setRole] = useState("admin");
  const data = useQuery({ queryKey: ["role-dashboard", role], queryFn: () => reportingApi.dashboard(role) });

  return (
    <div className="rounded-xl bg-white p-6 shadow-sm space-y-3">
      <h2 className="text-xl font-bold">Role-Based Dashboards</h2>
      <select className="rounded border px-3 py-2" value={role} onChange={(e) => setRole(e.target.value)}>
        <option>admin</option><option>reception</option><option>doctor</option><option>nurse</option><option>pharmacy</option><option>lab</option>
      </select>
      <div className="grid gap-2 md:grid-cols-3 text-sm">
        {(data.data?.kpis ?? []).map((k: any) => <div key={k.key} className="rounded border p-2"><p className="font-semibold">{k.label}</p><p>{k.value} {k.unit}</p></div>)}
      </div>
    </div>
  );
}
