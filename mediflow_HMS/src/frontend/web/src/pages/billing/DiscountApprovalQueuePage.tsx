import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { billingApi } from "@/lib/api";

export function DiscountApprovalQueuePage() {
  const qc = useQueryClient(); const q = useQuery({ queryKey: ["discount-queue"], queryFn: billingApi.discountQueue });
  const resolve = useMutation({ mutationFn: ({id,status}:{id:string;status:string}) => billingApi.resolveDiscount(id,status), onSuccess: ()=>qc.invalidateQueries({queryKey:["discount-queue"]}) });
  return <div className="rounded-xl bg-white p-6 shadow-sm"><h2 className="text-xl font-bold mb-3">Discount Approval Queue</h2><ul className="space-y-2 text-sm">{(q.data??[]).map((d)=><li key={d.id} className="rounded border p-2">Bill: {d.billId} | {d.amount} | {d.reason}<button className="ml-2 rounded bg-green-700 px-2 py-1 text-white" onClick={()=>resolve.mutate({id:d.id,status:"Approved"})}>Approve</button><button className="ml-2 rounded bg-red-700 px-2 py-1 text-white" onClick={()=>resolve.mutate({id:d.id,status:"Rejected"})}>Reject</button></li>)}</ul></div>;
}
