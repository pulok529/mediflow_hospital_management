import { useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { billingApi } from "@/lib/api";

export function DischargeChecklistPage() {
  const [form, setForm] = useState({ admissionId: "", patientName: "", diagnosis: "", treatmentSummary: "", advice: "", dischargeDate: new Date().toISOString().slice(0,10) });
  const save = useMutation({ mutationFn: () => billingApi.saveDischarge(form) });
  return <div className="rounded-xl bg-white p-6 shadow-sm space-y-3"><h2 className="text-xl font-bold">Discharge Checklist</h2>{Object.keys(form).map((k)=><input key={k} className="rounded border px-3 py-2 block w-full" value={(form as any)[k]} onChange={(e)=>setForm({...form,[k]:e.target.value})} placeholder={k} />)}<button className="rounded bg-slate-900 px-4 py-2 text-white" onClick={()=>save.mutate()}>Save Discharge Summary</button></div>;
}
