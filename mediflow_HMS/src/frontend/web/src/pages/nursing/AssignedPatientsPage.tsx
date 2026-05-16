import { useQuery } from "@tanstack/react-query";
import { nursingApi } from "@/lib/api";

export function AssignedPatientsPage() {
  const data = useQuery({ queryKey: ["nursing-dashboard"], queryFn: nursingApi.dashboard });
  return (
    <div className="rounded-xl bg-white p-6 shadow-sm">
      <h2 className="mb-3 text-xl font-bold">Assigned Patients</h2>
      <ul className="space-y-2 text-sm">
        {(data.data?.assignedPatients ?? []).map((p: any) => (
          <li key={p.id} className="rounded border p-2">{p.patientName} | Pending meds: {p.pendingMedications} | Pending vitals: {p.pendingVitals}</li>
        ))}
      </ul>
    </div>
  );
}
