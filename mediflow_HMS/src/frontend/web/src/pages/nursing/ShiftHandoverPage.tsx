import { useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { nursingApi } from "@/lib/api";

export function ShiftHandoverPage() {
  const [admissionId, setAdmissionId] = useState("");
  const [fromShift, setFromShift] = useState("Morning");
  const [toShift, setToShift] = useState("Evening");
  const [summary, setSummary] = useState("");
  const save = useMutation({ mutationFn: () => nursingApi.handover.add({ admissionId, fromShift, toShift, summary }) });

  return (
    <div className="rounded-xl bg-white p-6 shadow-sm space-y-3">
      <h2 className="text-xl font-bold">Shift Handover Screen</h2>
      <input className="rounded border px-3 py-2" placeholder="Admission ID" value={admissionId} onChange={(e) => setAdmissionId(e.target.value)} />
      <div className="grid gap-2 md:grid-cols-2">
        <input className="rounded border px-3 py-2" value={fromShift} onChange={(e) => setFromShift(e.target.value)} />
        <input className="rounded border px-3 py-2" value={toShift} onChange={(e) => setToShift(e.target.value)} />
      </div>
      <textarea className="rounded border px-3 py-2" value={summary} onChange={(e) => setSummary(e.target.value)} placeholder="Summary" />
      <button className="rounded bg-slate-900 px-4 py-2 text-white" onClick={() => save.mutate()}>Submit Handover</button>
    </div>
  );
}
