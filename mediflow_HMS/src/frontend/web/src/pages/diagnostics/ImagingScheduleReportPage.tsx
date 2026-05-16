import { useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { diagnosticsApi } from "@/lib/api";

export function ImagingScheduleReportPage() {
  const qc = useQueryClient();
  const orders = useQuery({ queryKey: ["imaging-orders"], queryFn: diagnosticsApi.imaging.orders });
  const [id, setId] = useState("");
  const [scheduledAtUtc, setScheduledAtUtc] = useState("");
  const [reportText, setReportText] = useState("");
  const [fileReference, setFileReference] = useState("");

  const schedule = useMutation({ mutationFn: () => diagnosticsApi.imaging.schedule(id, { scheduledAtUtc }), onSuccess: () => qc.invalidateQueries({ queryKey: ["imaging-orders"] }) });
  const report = useMutation({ mutationFn: () => diagnosticsApi.imaging.report(id, { reportText, fileReference }), onSuccess: () => qc.invalidateQueries({ queryKey: ["imaging-orders"] }) });
  const approve = useMutation({ mutationFn: () => diagnosticsApi.imaging.approve(id), onSuccess: () => qc.invalidateQueries({ queryKey: ["imaging-orders"] }) });

  return (
    <div className="rounded-xl bg-white p-6 shadow-sm space-y-3">
      <h2 className="text-xl font-bold">Imaging Schedule and Report</h2>
      <select className="rounded border px-3 py-2" value={id} onChange={(e) => setId(e.target.value)}>
        <option value="">Select imaging order</option>
        {(orders.data ?? []).map((o) => <option key={o.id} value={o.id}>{o.patientName} - {o.serviceName}</option>)}
      </select>
      <input className="rounded border px-3 py-2" type="datetime-local" value={scheduledAtUtc} onChange={(e) => setScheduledAtUtc(e.target.value)} />
      <button className="rounded bg-slate-700 px-4 py-2 text-white" onClick={() => schedule.mutate()}>Schedule</button>
      <textarea className="rounded border px-3 py-2" placeholder="Report text" value={reportText} onChange={(e) => setReportText(e.target.value)} />
      <input className="rounded border px-3 py-2" placeholder="File reference URL/path" value={fileReference} onChange={(e) => setFileReference(e.target.value)} />
      <div className="flex gap-2">
        <button className="rounded bg-slate-700 px-4 py-2 text-white" onClick={() => report.mutate()}>Enter Report</button>
        <button className="rounded bg-green-700 px-4 py-2 text-white" onClick={() => approve.mutate()}>Approve</button>
      </div>
    </div>
  );
}
