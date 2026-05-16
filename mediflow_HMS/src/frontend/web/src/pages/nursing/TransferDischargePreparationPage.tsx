import { useQuery } from "@tanstack/react-query";
import { nursingApi } from "@/lib/api";

export function TransferDischargePreparationPage() {
  const list = useQuery({ queryKey: ["transfer-discharge"], queryFn: nursingApi.transferDischarge });
  return (
    <div className="rounded-xl bg-white p-6 shadow-sm">
      <h2 className="mb-3 text-xl font-bold">Transfer / Discharge Preparation</h2>
      <ul className="space-y-2 text-sm">
        {(list.data ?? []).map((x: any) => <li key={x.bedId} className="rounded border p-2">{x.patientName ?? "Unknown"} | {String(x.status)} | {x.floor}/{x.room}</li>)}
      </ul>
    </div>
  );
}
