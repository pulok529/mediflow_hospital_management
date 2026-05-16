import { useQuery } from "@tanstack/react-query";
import { nursingApi } from "@/lib/api";

export function NurseStationDashboardPage() {
  const data = useQuery({ queryKey: ["nursing-dashboard"], queryFn: nursingApi.dashboard });
  return (
    <div className="space-y-4 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">Nurse Station Dashboard</h2>
      <p className="text-sm">Assigned: {(data.data?.assignedPatients ?? []).length}</p>
      <p className="text-sm">Medication Due: {(data.data?.medicationDue ?? []).length}</p>
      <p className="text-sm">Active Alerts: {(data.data?.alerts ?? []).length}</p>
    </div>
  );
}
