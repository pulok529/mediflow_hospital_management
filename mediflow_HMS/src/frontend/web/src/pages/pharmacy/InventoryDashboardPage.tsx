import { useQuery } from "@tanstack/react-query";
import { pharmacyApi } from "@/lib/api";

export function InventoryDashboardPage() {
  const prs = useQuery({ queryKey: ["prs"], queryFn: pharmacyApi.prs });
  const pos = useQuery({ queryKey: ["pos"], queryFn: pharmacyApi.pos });
  const grns = useQuery({ queryKey: ["grns"], queryFn: pharmacyApi.grns });
  const moves = useQuery({ queryKey: ["moves"], queryFn: pharmacyApi.movements });

  return (
    <div className="space-y-3 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">Inventory Dashboard</h2>
      <p className="text-sm">PR: {(prs.data ?? []).length}</p>
      <p className="text-sm">PO: {(pos.data ?? []).length}</p>
      <p className="text-sm">GRN: {(grns.data ?? []).length}</p>
      <p className="text-sm">Stock Movements: {(moves.data ?? []).length}</p>
    </div>
  );
}
