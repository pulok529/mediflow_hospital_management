import { useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { admissionApi } from "@/lib/api";

export function FloorReceptionDashboardPage() {
  const qc = useQueryClient();
  const data = useQuery({ queryKey: ["floor-reception"], queryFn: admissionApi.admissions.floorReception });
  const [admissionId, setAdmissionId] = useState("");
  const [toBedId, setToBedId] = useState("");
  const [reason, setReason] = useState("Clinical transfer");

  const transfer = useMutation({
    mutationFn: () => admissionApi.admissions.transferBed(admissionId, toBedId, reason),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["floor-reception"] });
      qc.invalidateQueries({ queryKey: ["bed-board"] });
    }
  });

  const admissions = data.data?.admissions ?? [];
  const board = data.data?.board ?? [];
  const availableBeds = board.filter((b: any) => b.status === "Available" || b.status === 0);

  return (
    <div className="space-y-4 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">Floor Reception Dashboard</h2>
      <div className="grid gap-2 md:grid-cols-3">
        <select className="rounded border px-3 py-2" value={admissionId} onChange={(e) => setAdmissionId(e.target.value)}>
          <option value="">Select admitted patient</option>
          {admissions.filter((a: any) => a.status === "Admitted").map((a: any) => <option key={a.id} value={a.id}>{a.patientName}</option>)}
        </select>
        <select className="rounded border px-3 py-2" value={toBedId} onChange={(e) => setToBedId(e.target.value)}>
          <option value="">Select target bed</option>
          {availableBeds.map((b: any) => <option key={b.bedId} value={b.bedId}>{b.bedNumber} ({b.floor}/{b.room})</option>)}
        </select>
        <input className="rounded border px-3 py-2" value={reason} onChange={(e) => setReason(e.target.value)} />
      </div>
      <button className="rounded bg-slate-900 px-4 py-2 text-white" onClick={() => transfer.mutate()}>Transfer Bed</button>

      <h3 className="font-semibold">Current Admissions</h3>
      <ul className="space-y-2 text-sm">
        {admissions.map((a: any) => <li key={a.id} className="rounded border p-2">{a.patientName} | {a.status} | Deposit: {a.depositAmount}</li>)}
      </ul>
    </div>
  );
}
