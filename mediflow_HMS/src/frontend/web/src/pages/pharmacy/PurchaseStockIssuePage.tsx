import { useState } from "react";
import { useMutation, useQuery } from "@tanstack/react-query";
import { pharmacyApi } from "@/lib/api";

export function PurchaseStockIssuePage() {
  const suppliers = useQuery({ queryKey: ["suppliers"], queryFn: pharmacyApi.suppliers });
  const [department, setDepartment] = useState("Pharmacy");
  const [supplierId, setSupplierId] = useState("");
  const [issue, setIssue] = useState({ department: "Ward-A", medicineCode: "", quantity: "1" });

  const createPr = useMutation({ mutationFn: () => pharmacyApi.createPr({ department }) });
  const createPo = useMutation({ mutationFn: () => pharmacyApi.createPo({ supplierId }) });
  const issueDept = useMutation({ mutationFn: () => pharmacyApi.issueDept({ department: issue.department, medicineCode: issue.medicineCode, quantity: Number(issue.quantity) }) });

  return (
    <div className="space-y-4 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">Purchase and Stock Issue</h2>
      <div className="space-y-2">
        <input className="rounded border px-3 py-2" value={department} onChange={(e) => setDepartment(e.target.value)} placeholder="Department for PR" />
        <button className="rounded bg-slate-900 px-4 py-2 text-white" onClick={() => createPr.mutate()}>Create Purchase Request</button>
      </div>
      <div className="space-y-2">
        <select className="rounded border px-3 py-2" value={supplierId} onChange={(e) => setSupplierId(e.target.value)}>
          <option value="">Select supplier</option>
          {(suppliers.data ?? []).map((s) => <option key={s.id} value={s.id}>{s.name}</option>)}
        </select>
        <button className="rounded bg-slate-900 px-4 py-2 text-white" onClick={() => createPo.mutate()}>Create Purchase Order</button>
      </div>
      <div className="space-y-2">
        <input className="rounded border px-3 py-2" value={issue.department} onChange={(e) => setIssue({ ...issue, department: e.target.value })} placeholder="Issue department" />
        <input className="rounded border px-3 py-2" value={issue.medicineCode} onChange={(e) => setIssue({ ...issue, medicineCode: e.target.value })} placeholder="Medicine code" />
        <input className="rounded border px-3 py-2" value={issue.quantity} onChange={(e) => setIssue({ ...issue, quantity: e.target.value })} placeholder="Quantity" />
        <button className="rounded bg-green-700 px-4 py-2 text-white" onClick={() => issueDept.mutate()}>Issue to Department</button>
      </div>
    </div>
  );
}
