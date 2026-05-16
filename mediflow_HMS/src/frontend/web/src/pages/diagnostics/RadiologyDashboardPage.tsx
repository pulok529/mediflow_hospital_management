import { useQuery } from "@tanstack/react-query";
import { diagnosticsApi } from "@/lib/api";

export function RadiologyDashboardPage() {
  const orders = useQuery({ queryKey: ["imaging-orders"], queryFn: diagnosticsApi.imaging.orders });
  return (
    <div className="rounded-xl bg-white p-6 shadow-sm space-y-3">
      <h2 className="text-xl font-bold">Radiology Dashboard</h2>
      <p className="text-sm">Total imaging orders: {(orders.data ?? []).length}</p>
    </div>
  );
}
