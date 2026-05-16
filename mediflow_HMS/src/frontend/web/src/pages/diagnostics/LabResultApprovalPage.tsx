import { useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { diagnosticsApi } from "@/lib/api";

export function LabResultApprovalPage() {
  const qc = useQueryClient();
  const orders = useQuery({ queryKey: ["lab-orders"], queryFn: diagnosticsApi.lab.orders });
  const [id, setId] = useState("");
  const [resultValue, setResultValue] = useState("");
  const [resultNotes, setResultNotes] = useState("");
  const [isCritical, setIsCritical] = useState(false);

  const enter = useMutation({ mutationFn: () => diagnosticsApi.lab.result(id, { resultValue, resultNotes, isCritical }), onSuccess: () => qc.invalidateQueries({ queryKey: ["lab-orders"] }) });
  const approve = useMutation({ mutationFn: () => diagnosticsApi.lab.approve(id), onSuccess: () => qc.invalidateQueries({ queryKey: ["lab-orders"] }) });

  return (
    <div className="rounded-xl bg-white p-6 shadow-sm space-y-3">
      <h2 className="text-xl font-bold">Result Entry and Approval</h2>
      <select className="rounded border px-3 py-2" value={id} onChange={(e) => setId(e.target.value)}>
        <option value="">Select lab order</option>
        {(orders.data ?? []).map((o) => <option key={o.id} value={o.id}>{o.patientName} - {o.testName}</option>)}
      </select>
      <input className="rounded border px-3 py-2" placeholder="Result value" value={resultValue} onChange={(e) => setResultValue(e.target.value)} />
      <input className="rounded border px-3 py-2" placeholder="Result notes" value={resultNotes} onChange={(e) => setResultNotes(e.target.value)} />
      <label className="text-sm"><input type="checkbox" checked={isCritical} onChange={(e) => setIsCritical(e.target.checked)} /> Critical result</label>
      <div className="flex gap-2">
        <button className="rounded bg-slate-700 px-4 py-2 text-white" onClick={() => enter.mutate()}>Enter Result</button>
        <button className="rounded bg-green-700 px-4 py-2 text-white" onClick={() => approve.mutate()}>Approve</button>
      </div>
    </div>
  );
}
