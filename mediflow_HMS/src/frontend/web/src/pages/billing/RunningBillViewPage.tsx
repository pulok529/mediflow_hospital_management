import { useState } from "react";
import { useMutation, useQuery } from "@tanstack/react-query";
import { billingApi } from "@/lib/api";

export function RunningBillViewPage() {
  const bills = useQuery({ queryKey: ["bills"], queryFn: billingApi.bills });
  const [billId, setBillId] = useState("");
  const [item, setItem] = useState("");
  const [amount, setAmount] = useState("0");
  const add = useMutation({ mutationFn: () => billingApi.addRunningLine({ billId, line: { item, amount: Number(amount) } }) });
  return <div className="rounded-xl bg-white p-6 shadow-sm space-y-3"><h2 className="text-xl font-bold">Running Bill View</h2><select className="rounded border px-3 py-2" value={billId} onChange={(e)=>setBillId(e.target.value)}><option value="">Select bill</option>{(bills.data??[]).map((b)=><option key={b.id} value={b.id}>{b.patientName}</option>)}</select><input className="rounded border px-3 py-2" placeholder="Item" value={item} onChange={(e)=>setItem(e.target.value)}/><input className="rounded border px-3 py-2" placeholder="Amount" value={amount} onChange={(e)=>setAmount(e.target.value)}/><button className="rounded bg-slate-900 px-4 py-2 text-white" onClick={()=>add.mutate()}>Add Line</button></div>;
}
