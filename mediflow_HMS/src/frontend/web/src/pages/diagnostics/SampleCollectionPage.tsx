import { useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { diagnosticsApi } from "@/lib/api";

export function SampleCollectionPage() {
  const qc = useQueryClient();
  const orders = useQuery({ queryKey: ["lab-orders"], queryFn: diagnosticsApi.lab.orders });
  const collect = useMutation({ mutationFn: (id: string) => diagnosticsApi.lab.collect(id), onSuccess: () => qc.invalidateQueries({ queryKey: ["lab-orders"] }) });
  const [selected, setSelected] = useState("");

  return (
    <div className="rounded-xl bg-white p-6 shadow-sm space-y-3">
      <h2 className="text-xl font-bold">Sample Collection Screen</h2>
      <select className="rounded border px-3 py-2" value={selected} onChange={(e) => setSelected(e.target.value)}>
        <option value="">Select lab order</option>
        {(orders.data ?? []).map((o) => <option key={o.id} value={o.id}>{o.patientName} - {o.testName}</option>)}
      </select>
      <button className="rounded bg-slate-900 px-4 py-2 text-white" onClick={() => collect.mutate(selected)}>Mark Sample Collected</button>
    </div>
  );
}
