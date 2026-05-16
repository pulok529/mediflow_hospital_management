import { useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { admissionApi } from "@/lib/api";

export function AdmissionDeskWizardPage() {
  const qc = useQueryClient();
  const [patientId, setPatientId] = useState("");
  const [patientName, setPatientName] = useState("");
  const [source, setSource] = useState("OPD");
  const [depositAmount, setDepositAmount] = useState("0");
  const [depositReference, setDepositReference] = useState("");
  const [selectedAdmission, setSelectedAdmission] = useState("");
  const [bedId, setBedId] = useState("");

  const admissions = useQuery({ queryKey: ["admissions"], queryFn: admissionApi.admissions.list });
  const board = useQuery({ queryKey: ["bed-board"], queryFn: admissionApi.admissions.bedBoard });

  const create = useMutation({
    mutationFn: () => admissionApi.admissions.create({ patientId, patientName, source, depositAmount: Number(depositAmount), depositReference }),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["admissions"] })
  });

  const assign = useMutation({
    mutationFn: () => admissionApi.admissions.assignBed(selectedAdmission, bedId),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["admissions"] });
      qc.invalidateQueries({ queryKey: ["bed-board"] });
    }
  });

  return (
    <div className="space-y-4 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">Admission Desk Wizard</h2>
      <div className="grid gap-2 md:grid-cols-3">
        <input className="rounded border px-3 py-2" placeholder="Patient ID" value={patientId} onChange={(e) => setPatientId(e.target.value)} />
        <input className="rounded border px-3 py-2" placeholder="Patient Name" value={patientName} onChange={(e) => setPatientName(e.target.value)} />
        <select className="rounded border px-3 py-2" value={source} onChange={(e) => setSource(e.target.value)}>
          <option>OPD</option><option>Emergency</option>
        </select>
      </div>
      <div className="grid gap-2 md:grid-cols-2">
        <input className="rounded border px-3 py-2" placeholder="Deposit amount" value={depositAmount} onChange={(e) => setDepositAmount(e.target.value)} />
        <input className="rounded border px-3 py-2" placeholder="Deposit reference" value={depositReference} onChange={(e) => setDepositReference(e.target.value)} />
      </div>
      <button className="rounded bg-slate-900 px-4 py-2 text-white" onClick={() => create.mutate()}>Step 1: Create Admission Request</button>

      <h3 className="font-semibold">Step 2: Assign Bed</h3>
      <div className="grid gap-2 md:grid-cols-2">
        <select className="rounded border px-3 py-2" value={selectedAdmission} onChange={(e) => setSelectedAdmission(e.target.value)}>
          <option value="">Select admission</option>
          {(admissions.data ?? []).map((a) => <option key={a.id} value={a.id}>{a.patientName} ({a.status})</option>)}
        </select>
        <select className="rounded border px-3 py-2" value={bedId} onChange={(e) => setBedId(e.target.value)}>
          <option value="">Select available bed</option>
          {(board.data ?? []).filter((b) => b.status === "Available" || b.status === 0).map((b) => <option key={b.bedId} value={b.bedId}>{b.bedNumber} - {b.room}</option>)}
        </select>
      </div>
      <button className="rounded bg-green-700 px-4 py-2 text-white" onClick={() => assign.mutate()}>Assign Bed</button>
    </div>
  );
}
