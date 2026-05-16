import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { consultationApi } from "@/lib/api";

export function HistoryTimelinePage() {
  const [patientId, setPatientId] = useState("");
  const history = useQuery({ queryKey: ["history", patientId], queryFn: () => consultationApi.history(patientId), enabled: !!patientId });

  return (
    <div className="space-y-3 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">Previous History Timeline</h2>
      <input className="rounded border px-3 py-2" placeholder="Patient ID" value={patientId} onChange={(e) => setPatientId(e.target.value)} />
      <ul className="space-y-2 text-sm">
        {(history.data ?? []).map((h) => (
          <li key={h.id} className="rounded border p-2">
            <p>{h.startedAtUtc} - {h.diagnosis}</p>
            <p className="text-slate-500">Complaint: {h.chiefComplaint}</p>
          </li>
        ))}
      </ul>
    </div>
  );
}
