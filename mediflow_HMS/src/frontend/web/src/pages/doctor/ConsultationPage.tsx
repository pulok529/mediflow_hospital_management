import { useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { consultationApi } from "@/lib/api";

export function ConsultationPage() {
  const [encounterId, setEncounterId] = useState("");
  const [patientId, setPatientId] = useState("");
  const [patientName, setPatientName] = useState("");
  const [appointmentId, setAppointmentId] = useState("");
  const [chiefComplaint, setChiefComplaint] = useState("");
  const [diagnosis, setDiagnosis] = useState("");
  const [clinicalNotes, setClinicalNotes] = useState("");
  const [followUpDate, setFollowUpDate] = useState("");
  const [followUpAdvice, setFollowUpAdvice] = useState("");
  const [vitals, setVitals] = useState({ temperatureC: "", pulse: "", systolicBp: "", diastolicBp: "", respiratoryRate: "", spO2: "", weightKg: "", heightCm: "" });
  const [med, setMed] = useState({ medicineName: "", dose: "", frequency: "", duration: "", instructions: "" });
  const [order, setOrder] = useState({ type: "Lab", testOrProcedure: "", notes: "" });

  const start = useMutation({ mutationFn: () => consultationApi.start({ appointmentId, patientId, patientName }), onSuccess: (d) => setEncounterId(d.id) });
  const save = useMutation({
    mutationFn: (complete: boolean) => consultationApi.save(encounterId, {
      vitals: {
        temperatureC: vitals.temperatureC ? Number(vitals.temperatureC) : null,
        pulse: vitals.pulse ? Number(vitals.pulse) : null,
        systolicBp: vitals.systolicBp ? Number(vitals.systolicBp) : null,
        diastolicBp: vitals.diastolicBp ? Number(vitals.diastolicBp) : null,
        respiratoryRate: vitals.respiratoryRate ? Number(vitals.respiratoryRate) : null,
        spO2: vitals.spO2 ? Number(vitals.spO2) : null,
        weightKg: vitals.weightKg ? Number(vitals.weightKg) : null,
        heightCm: vitals.heightCm ? Number(vitals.heightCm) : null
      },
      chiefComplaint,
      diagnosis,
      clinicalNotes,
      prescriptions: med.medicineName ? [med] : [],
      orders: order.testOrProcedure ? [order] : [],
      followUpDate: followUpDate || null,
      followUpAdvice,
      completeEncounter: complete
    })
  });

  return (
    <div className="space-y-4 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">Consultation Screen</h2>
      <div className="grid gap-2 md:grid-cols-3">
        <input className="rounded border px-3 py-2" placeholder="Appointment ID" value={appointmentId} onChange={(e) => setAppointmentId(e.target.value)} />
        <input className="rounded border px-3 py-2" placeholder="Patient ID" value={patientId} onChange={(e) => setPatientId(e.target.value)} />
        <input className="rounded border px-3 py-2" placeholder="Patient Name" value={patientName} onChange={(e) => setPatientName(e.target.value)} />
      </div>
      <button className="rounded bg-slate-900 px-4 py-2 text-white" onClick={() => start.mutate()}>Start Encounter</button>
      <p className="text-xs text-slate-500">Encounter ID: {encounterId || "N/A"}</p>

      <h3 className="font-semibold">Vitals Panel</h3>
      <div className="grid gap-2 md:grid-cols-4">
        {Object.keys(vitals).map((k) => (
          <input key={k} className="rounded border px-3 py-2" placeholder={k} value={(vitals as any)[k]} onChange={(e) => setVitals({ ...vitals, [k]: e.target.value })} />
        ))}
      </div>

      <h3 className="font-semibold">Clinical Notes</h3>
      <input className="w-full rounded border px-3 py-2" placeholder="Chief complaint" value={chiefComplaint} onChange={(e) => setChiefComplaint(e.target.value)} />
      <input className="w-full rounded border px-3 py-2" placeholder="Diagnosis" value={diagnosis} onChange={(e) => setDiagnosis(e.target.value)} />
      <textarea className="w-full rounded border px-3 py-2" placeholder="Clinical notes" value={clinicalNotes} onChange={(e) => setClinicalNotes(e.target.value)} />

      <h3 className="font-semibold">Prescription Builder</h3>
      <div className="grid gap-2 md:grid-cols-5">
        {Object.keys(med).map((k) => (
          <input key={k} className="rounded border px-3 py-2" placeholder={k} value={(med as any)[k]} onChange={(e) => setMed({ ...med, [k]: e.target.value })} />
        ))}
      </div>

      <h3 className="font-semibold">Doctor Orders (Lab/Radiology)</h3>
      <div className="grid gap-2 md:grid-cols-3">
        <select className="rounded border px-3 py-2" value={order.type} onChange={(e) => setOrder({ ...order, type: e.target.value })}>
          <option>Lab</option>
          <option>Radiology</option>
        </select>
        <input className="rounded border px-3 py-2" placeholder="testOrProcedure" value={order.testOrProcedure} onChange={(e) => setOrder({ ...order, testOrProcedure: e.target.value })} />
        <input className="rounded border px-3 py-2" placeholder="notes" value={order.notes} onChange={(e) => setOrder({ ...order, notes: e.target.value })} />
      </div>

      <h3 className="font-semibold">Follow-up / Advice</h3>
      <input type="date" className="rounded border px-3 py-2" value={followUpDate} onChange={(e) => setFollowUpDate(e.target.value)} />
      <textarea className="w-full rounded border px-3 py-2" placeholder="Follow-up advice" value={followUpAdvice} onChange={(e) => setFollowUpAdvice(e.target.value)} />

      <div className="flex gap-2">
        <button className="rounded bg-slate-700 px-4 py-2 text-white" onClick={() => save.mutate(false)}>Save Draft</button>
        <button className="rounded bg-green-700 px-4 py-2 text-white" onClick={() => save.mutate(true)}>Complete Consultation</button>
      </div>
    </div>
  );
}
