import { useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { workflowApi } from "@/lib/api";

export function AppointmentBookingPage() {
  const [form, setForm] = useState({ patientId: "", appointmentDate: "", department: "", doctorName: "" });
  const [validationError, setValidationError] = useState("");
  const create = useMutation({ mutationFn: () => workflowApi.appointments.create(form) });
  const onBook = () => {
    if (!form.patientId || !form.appointmentDate || !form.department || !form.doctorName) {
      setValidationError("Please provide patient ID, date, department, and doctor name.");
      return;
    }
    setValidationError("");
    create.mutate();
  };

  return (
    <div className="space-y-3 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">Appointment Booking</h2>
      <p className="text-xs text-slate-500">Paste patient ID from search result, then book.</p>
      <input placeholder="Patient ID*" className="block w-full rounded border px-3 py-2 focus:border-slate-700 focus:outline-none" value={form.patientId} onChange={(e) => setForm({ ...form, patientId: e.target.value })} />
      <input placeholder="Appointment Date (YYYY-MM-DD)*" className="block w-full rounded border px-3 py-2 focus:border-slate-700 focus:outline-none" value={form.appointmentDate} onChange={(e) => setForm({ ...form, appointmentDate: e.target.value })} />
      <input placeholder="Department*" className="block w-full rounded border px-3 py-2 focus:border-slate-700 focus:outline-none" value={form.department} onChange={(e) => setForm({ ...form, department: e.target.value })} />
      <input placeholder="Doctor Name*" className="block w-full rounded border px-3 py-2 focus:border-slate-700 focus:outline-none" value={form.doctorName} onChange={(e) => setForm({ ...form, doctorName: e.target.value })} />
      {validationError ? <p className="text-sm text-red-600">{validationError}</p> : null}
      {create.error ? <p className="text-sm text-red-600">{create.error instanceof Error ? create.error.message : "Booking failed."}</p> : null}
      <button className="rounded bg-slate-900 px-4 py-2 text-white disabled:opacity-60" disabled={create.isPending} onClick={onBook}>{create.isPending ? "Booking..." : "Book"}</button>
      {create.data ? <p className="text-sm text-green-700">Token: {create.data.tokenNumber}</p> : null}
    </div>
  );
}
