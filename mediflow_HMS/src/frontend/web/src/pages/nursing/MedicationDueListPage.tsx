import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { nursingApi } from "@/lib/api";

export function MedicationDueListPage() {
  const qc = useQueryClient();
  const due = useQuery({ queryKey: ["med-due"], queryFn: nursingApi.medications.due });
  const administer = useMutation({ mutationFn: (id: string) => nursingApi.medications.administer(id), onSuccess: () => qc.invalidateQueries({ queryKey: ["med-due"] }) });

  return (
    <div className="rounded-xl bg-white p-6 shadow-sm">
      <h2 className="mb-3 text-xl font-bold">Medication Due List</h2>
      <ul className="space-y-2 text-sm">
        {(due.data ?? []).map((m: any) => (
          <li key={m.id} className="rounded border p-2">
            {m.medicationName} ({m.dose}) due {m.dueAtUtc}
            <button className="ml-3 rounded bg-green-700 px-2 py-1 text-xs text-white" onClick={() => administer.mutate(m.id)}>Administer</button>
          </li>
        ))}
      </ul>
    </div>
  );
}
