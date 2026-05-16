import { useState } from "react";
import { useMutation, useQuery } from "@tanstack/react-query";
import { billingApi } from "@/lib/api";

export function PaymentEntryPage() {
  const bills = useQuery({ queryKey: ["bills"], queryFn: billingApi.bills });
  const [billId, setBillId] = useState(""); const [amount, setAmount] = useState("0"); const [mode, setMode] = useState("Cash"); const [isAdvance, setIsAdvance] = useState(false);
  const save = useMutation({ mutationFn: () => billingApi.addPayment({ billId, amount: Number(amount), mode, isAdvance }) });
  return <div className="rounded-xl bg-white p-6 shadow-sm space-y-3"><h2 className="text-xl font-bold">Payment Entry</h2><select className="rounded border px-3 py-2" value={billId} onChange={(e)=>setBillId(e.target.value)}><option value="">Select bill</option>{(bills.data??[]).map((b)=><option key={b.id} value={b.id}>{b.patientName}</option>)}</select><input className="rounded border px-3 py-2" value={amount} onChange={(e)=>setAmount(e.target.value)} placeholder="Amount"/><input className="rounded border px-3 py-2" value={mode} onChange={(e)=>setMode(e.target.value)} placeholder="Mode"/><label><input type="checkbox" checked={isAdvance} onChange={(e)=>setIsAdvance(e.target.checked)}/> Advance</label><button className="rounded bg-slate-900 px-4 py-2 text-white" onClick={()=>save.mutate()}>Save Payment</button></div>;
}
