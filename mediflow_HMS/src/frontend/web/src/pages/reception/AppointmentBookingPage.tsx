import { useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { workflowApi } from "@/lib/api";

export function AppointmentBookingPage() {
  const [form, setForm] = useState({ patientId: "", appointmentDate: "", department: "", doctorName: "" });
  const create = useMutation({ mutationFn: () => workflowApi.appointments.create(form) });

  return (
    <div className="space-y-3 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">Appointment Booking</h2>
      <p className="text-xs text-slate-500">Paste patient ID from search result, then book.</p>
      {Object.keys(form).map((k) => (
        <input key={k} placeholder={k} className="block w-full rounded border px-3 py-2" value={(form as any)[k]} onChange={(e) => setForm({ ...form, [k]: e.target.value })} />
      ))}
      <button className="rounded bg-slate-900 px-4 py-2 text-white" onClick={() => create.mutate()}>Book</button>
      {create.data ? <p className="text-sm text-green-700">Token: {create.data.tokenNumber}</p> : null}
    </div>
  );
}
