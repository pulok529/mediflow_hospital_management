import { useState } from "react";
import { useMutation, useQuery } from "@tanstack/react-query";
import { billingApi } from "@/lib/api";

export function FinalBillClearancePage() {
  const bills = useQuery({ queryKey: ["bills"], queryFn: billingApi.bills }); const [billId,setBillId]=useState("");
  const clear = useMutation({ mutationFn: () => billingApi.finalClearance({ billId }) });
  return <div className="rounded-xl bg-white p-6 shadow-sm space-y-3"><h2 className="text-xl font-bold">Final Bill Clearance</h2><select className="rounded border px-3 py-2" value={billId} onChange={(e)=>setBillId(e.target.value)}><option value="">Select bill</option>{(bills.data??[]).map((b)=><option key={b.id} value={b.id}>{b.patientName}</option>)}</select><button className="rounded bg-green-700 px-4 py-2 text-white" onClick={()=>clear.mutate()}>Clear Bill</button></div>;
}
