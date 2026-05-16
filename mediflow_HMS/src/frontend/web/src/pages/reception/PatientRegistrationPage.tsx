import { useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { workflowApi } from "@/lib/api";

export function PatientRegistrationPage() {
  const [form, setForm] = useState({ fullName: "", dateOfBirth: "", phone: "", gender: "", address: "" });
  const [validationError, setValidationError] = useState("");
  const create = useMutation({ mutationFn: () => workflowApi.patients.create(form) });
  const onRegister = () => {
    if (!form.fullName || !form.dateOfBirth || !form.phone || !form.gender) {
      setValidationError("Please complete full name, date of birth, phone, and gender.");
      return;
    }
    setValidationError("");
    create.mutate();
  };

  return (
    <div className="space-y-3 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">New Patient Registration</h2>
      <p className="text-xs text-slate-500">All mandatory fields must be completed before registration.</p>
      <div className="grid gap-2 md:grid-cols-2">
        <input placeholder="Full Name*" className="rounded border px-3 py-2 focus:border-slate-700 focus:outline-none" value={form.fullName} onChange={(e) => setForm({ ...form, fullName: e.target.value })} />
        <input placeholder="Date of Birth (YYYY-MM-DD)*" className="rounded border px-3 py-2 focus:border-slate-700 focus:outline-none" value={form.dateOfBirth} onChange={(e) => setForm({ ...form, dateOfBirth: e.target.value })} />
        <input placeholder="Phone*" className="rounded border px-3 py-2 focus:border-slate-700 focus:outline-none" value={form.phone} onChange={(e) => setForm({ ...form, phone: e.target.value })} />
        <input placeholder="Gender*" className="rounded border px-3 py-2 focus:border-slate-700 focus:outline-none" value={form.gender} onChange={(e) => setForm({ ...form, gender: e.target.value })} />
        <input placeholder="Address" className="rounded border px-3 py-2 focus:border-slate-700 focus:outline-none md:col-span-2" value={form.address} onChange={(e) => setForm({ ...form, address: e.target.value })} />
      </div>
      {validationError ? <p className="text-sm text-red-600">{validationError}</p> : null}
      {create.error ? <p className="text-sm text-red-600">{create.error instanceof Error ? create.error.message : "Registration failed."}</p> : null}
      <button className="rounded bg-slate-900 px-4 py-2 text-white disabled:opacity-60" disabled={create.isPending} onClick={onRegister}>{create.isPending ? "Registering..." : "Register"}</button>
      {create.data ? <p className="text-sm text-green-700">Patient code: {create.data.patientCode}</p> : null}
    </div>
  );
}
