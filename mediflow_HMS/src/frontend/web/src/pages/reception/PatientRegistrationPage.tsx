import { useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { workflowApi } from "@/lib/api";

export function PatientRegistrationPage() {
  const [form, setForm] = useState({ fullName: "", dateOfBirth: "", phone: "", gender: "", address: "" });
  const create = useMutation({ mutationFn: () => workflowApi.patients.create(form) });

  return (
    <div className="space-y-3 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">New Patient Registration</h2>
      <div className="grid gap-2 md:grid-cols-2">
        {Object.keys(form).map((k) => (
          <input key={k} placeholder={k} className="rounded border px-3 py-2" value={(form as any)[k]} onChange={(e) => setForm({ ...form, [k]: e.target.value })} />
        ))}
      </div>
      <button className="rounded bg-slate-900 px-4 py-2 text-white" onClick={() => create.mutate()}>Register</button>
      {create.data ? <p className="text-sm text-green-700">Patient code: {create.data.patientCode}</p> : null}
    </div>
  );
}
