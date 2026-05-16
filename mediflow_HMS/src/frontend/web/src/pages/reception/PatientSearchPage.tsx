import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { workflowApi } from "@/lib/api";

export function PatientSearchPage() {
  const [q, setQ] = useState("");
  const params = new URLSearchParams();
  if (q) params.set("name", q);
  const result = useQuery({ queryKey: ["patient-search", q], queryFn: () => workflowApi.patients.search(params) });

  return (
    <div className="space-y-3 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">Existing Patient Search</h2>
      <input className="rounded border px-3 py-2" placeholder="Search by name" value={q} onChange={(e) => setQ(e.target.value)} />
      <ul className="space-y-2 text-sm">
        {(result.data ?? []).map((p) => <li key={p.id} className="rounded border p-2">{p.patientCode} - {p.fullName} - {p.phone}</li>)}
      </ul>
    </div>
  );
}
