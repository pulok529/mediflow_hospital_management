import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import { workflowApi } from "@/lib/api";

export function DailyAppointmentListPage() {
  const today = useMemo(() => new Date().toISOString().slice(0, 10), []);
  const list = useQuery({ queryKey: ["daily-appointments", today], queryFn: () => workflowApi.appointments.daily(today) });

  return (
    <div className="space-y-3 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">Daily Appointment List</h2>
      <ul className="space-y-2 text-sm">
        {(list.data ?? []).map((a) => <li key={a.id} className="rounded border p-2">#{a.tokenNumber} {a.patientName} - {a.department} ({a.state})</li>)}
      </ul>
    </div>
  );
}
