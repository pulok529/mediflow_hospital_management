import { useQuery } from "@tanstack/react-query";
import { pharmacyApi } from "@/lib/api";

export function PharmacyDashboardPage() {
  const data = useQuery({ queryKey: ["pharmacy-dashboard"], queryFn: pharmacyApi.dashboard });
  return (
    <div className="space-y-4 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">Pharmacy Dashboard</h2>
      <p className="text-sm">Medicines: {(data.data?.medicines ?? []).length}</p>
      <p className="text-sm">Batches: {(data.data?.batches ?? []).length}</p>
      <p className="text-sm">Low stock alerts: {(data.data?.lowStockAlerts ?? []).length}</p>
    </div>
  );
}
