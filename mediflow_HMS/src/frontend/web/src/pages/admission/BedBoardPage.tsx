import { useQuery } from "@tanstack/react-query";
import { admissionApi } from "@/lib/api";

const statusColor: Record<string, string> = {
  Available: "bg-green-100 text-green-800",
  Occupied: "bg-red-100 text-red-800",
  Reserved: "bg-yellow-100 text-yellow-800",
  CleaningRequired: "bg-orange-100 text-orange-800",
  UnderCleaning: "bg-orange-200 text-orange-900",
  Maintenance: "bg-gray-200 text-gray-900",
  Isolation: "bg-purple-100 text-purple-800",
  TransferPending: "bg-blue-100 text-blue-800",
  DischargePending: "bg-indigo-100 text-indigo-800"
};

export function BedBoardPage() {
  const board = useQuery({ queryKey: ["bed-board"], queryFn: admissionApi.admissions.bedBoard });

  return (
    <div className="space-y-4 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">Live Bed Board</h2>
      <div className="grid gap-3 md:grid-cols-3 lg:grid-cols-4">
        {(board.data ?? []).map((b) => {
          const status = String(b.status);
          return (
            <div key={b.bedId} className="rounded-lg border p-3">
              <p className="font-semibold">{b.building} / {b.floor}</p>
              <p className="text-sm">{b.ward} - {b.room} - Bed {b.bedNumber}</p>
              <p className={`mt-2 inline-block rounded px-2 py-1 text-xs ${statusColor[status] ?? "bg-slate-100"}`}>{status}</p>
              <p className="mt-2 text-xs text-slate-600">{b.patientName ?? "No patient assigned"}</p>
            </div>
          );
        })}
      </div>
    </div>
  );
}
