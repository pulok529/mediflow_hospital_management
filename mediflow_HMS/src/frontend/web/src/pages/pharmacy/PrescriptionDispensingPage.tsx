import { useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { pharmacyApi } from "@/lib/api";

export function PrescriptionDispensingPage() {
  const [mode, setMode] = useState("OPD");
  const [form, setForm] = useState({ encounterId: "", admissionId: "", patientName: "", medicineCode: "", quantity: "1" });

  const mutate = useMutation({ mutationFn: () => mode === "OPD"
    ? pharmacyApi.opdDispense({ encounterId: form.encounterId || null, patientName: form.patientName, medicineCode: form.medicineCode, quantity: Number(form.quantity) })
    : pharmacyApi.ipdIssue({ admissionId: form.admissionId, patientName: form.patientName, medicineCode: form.medicineCode, quantity: Number(form.quantity) }) });

  return (
    <div className="rounded-xl bg-white p-6 shadow-sm space-y-3">
      <h2 className="text-xl font-bold">Prescription Dispensing</h2>
      <select className="rounded border px-3 py-2" value={mode} onChange={(e) => setMode(e.target.value)}><option>OPD</option><option>IPD</option></select>
      {mode === "OPD" ? <input className="rounded border px-3 py-2" placeholder="Encounter ID" value={form.encounterId} onChange={(e) => setForm({ ...form, encounterId: e.target.value })} /> : <input className="rounded border px-3 py-2" placeholder="Admission ID" value={form.admissionId} onChange={(e) => setForm({ ...form, admissionId: e.target.value })} />}
      <input className="rounded border px-3 py-2" placeholder="Patient name" value={form.patientName} onChange={(e) => setForm({ ...form, patientName: e.target.value })} />
      <input className="rounded border px-3 py-2" placeholder="Medicine code" value={form.medicineCode} onChange={(e) => setForm({ ...form, medicineCode: e.target.value })} />
      <input className="rounded border px-3 py-2" placeholder="Quantity" value={form.quantity} onChange={(e) => setForm({ ...form, quantity: e.target.value })} />
      <button className="rounded bg-slate-900 px-4 py-2 text-white" onClick={() => mutate.mutate()}>Dispense / Issue</button>
    </div>
  );
}
