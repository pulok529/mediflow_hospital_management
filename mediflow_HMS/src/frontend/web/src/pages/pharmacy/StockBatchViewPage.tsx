import { useQuery } from "@tanstack/react-query";
import { pharmacyApi } from "@/lib/api";

export function StockBatchViewPage() {
  const batches = useQuery({ queryKey: ["batches"], queryFn: pharmacyApi.batches });
  const low = useQuery({ queryKey: ["low-stock"], queryFn: pharmacyApi.lowStock });

  return (
    <div className="space-y-4 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">Stock / Batch View</h2>
      <h3 className="font-semibold">Low Stock Alerts</h3>
      <ul className="text-sm space-y-1">{(low.data ?? []).map((x) => <li key={x.medicineId}>{x.medicineCode}: {x.totalStock} (reorder {x.reorderLevel})</li>)}</ul>
      <h3 className="font-semibold">Batches</h3>
      <ul className="text-sm space-y-1">{(batches.data ?? []).map((b) => <li key={b.id}>{b.batchNo} | Exp: {b.expiryDate} | Qty: {b.quantityOnHand}</li>)}</ul>
    </div>
  );
}
