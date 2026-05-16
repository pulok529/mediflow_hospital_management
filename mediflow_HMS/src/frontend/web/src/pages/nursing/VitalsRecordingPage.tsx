import { useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { nursingApi } from "@/lib/api";

export function VitalsRecordingPage() {
  const [vitalsScheduleId, setVitalsScheduleId] = useState("");
  const [form, setForm] = useState({ temperatureC: "", pulse: "", systolicBp: "", diastolicBp: "", spO2: "" });
  const record = useMutation({ mutationFn: () => nursingApi.vitals.record(vitalsScheduleId, {
    temperatureC: form.temperatureC ? Number(form.temperatureC) : null,
    pulse: form.pulse ? Number(form.pulse) : null,
    systolicBp: form.systolicBp ? Number(form.systolicBp) : null,
    diastolicBp: form.diastolicBp ? Number(form.diastolicBp) : null,
    spO2: form.spO2 ? Number(form.spO2) : null
  })});

  return (
    <div className="rounded-xl bg-white p-6 shadow-sm space-y-3">
      <h2 className="text-xl font-bold">Vitals Recording Screen</h2>
      <input className="rounded border px-3 py-2" placeholder="Vitals schedule ID" value={vitalsScheduleId} onChange={(e) => setVitalsScheduleId(e.target.value)} />
      <div className="grid gap-2 md:grid-cols-5">
        {Object.keys(form).map((k) => <input key={k} className="rounded border px-3 py-2" placeholder={k} value={(form as any)[k]} onChange={(e) => setForm({ ...form, [k]: e.target.value })} />)}
      </div>
      <button className="rounded bg-slate-900 px-4 py-2 text-white" onClick={() => record.mutate()}>Record Vitals</button>
    </div>
  );
}
