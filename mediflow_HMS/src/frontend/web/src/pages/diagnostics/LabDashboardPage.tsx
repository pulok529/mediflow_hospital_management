import { useQuery } from "@tanstack/react-query";
import { diagnosticsApi } from "@/lib/api";

export function LabDashboardPage() {
  const orders = useQuery({ queryKey: ["lab-orders"], queryFn: diagnosticsApi.lab.orders });
  const grouped = {
    ordered: (orders.data ?? []).filter((x) => String(x.status) === "Ordered" || x.status === 0),
    collected: (orders.data ?? []).filter((x) => String(x.status) === "SampleCollected" || x.status === 1),
    entered: (orders.data ?? []).filter((x) => String(x.status) === "ResultEntered" || x.status === 2),
    approved: (orders.data ?? []).filter((x) => String(x.status) === "Approved" || x.status === 3)
  };

  return (
    <div className="space-y-4 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">Lab Dashboard</h2>
      <div className="grid gap-3 md:grid-cols-4 text-sm">
        <div className="rounded border p-3">Ordered: {grouped.ordered.length}</div>
        <div className="rounded border p-3">Collected: {grouped.collected.length}</div>
        <div className="rounded border p-3">Result Entered: {grouped.entered.length}</div>
        <div className="rounded border p-3">Approved: {grouped.approved.length}</div>
      </div>
    </div>
  );
}
