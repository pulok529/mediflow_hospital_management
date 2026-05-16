import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import { consultationApi } from "@/lib/api";

export function WaitingPatientsPage() {
  const today = useMemo(() => new Date().toISOString().slice(0, 10), []);
  const waiting = useQuery({ queryKey: ["waiting", today], queryFn: () => consultationApi.waiting(today) });

  return (
    <div className="space-y-3 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">Waiting Patients List</h2>
      <ul className="space-y-2 text-sm">
        {(waiting.data ?? []).map((w) => (
          <li key={w.id} className="rounded border p-2">
            {w.patientName} | Encounter: {w.id}
          </li>
        ))}
      </ul>
    </div>
  );
}
