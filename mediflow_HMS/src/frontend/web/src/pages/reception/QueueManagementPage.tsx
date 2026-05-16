import { useMemo } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { workflowApi } from "@/lib/api";

export function QueueManagementPage() {
  const qc = useQueryClient();
  const today = useMemo(() => new Date().toISOString().slice(0, 10), []);
  const queue = useQuery({ queryKey: ["queue", today], queryFn: () => workflowApi.queue.list(today) });
  const update = useMutation({
    mutationFn: ({ id, state }: { id: string; state: string }) => workflowApi.queue.updateState(id, state),
    onSuccess: () => qc.invalidateQueries({ queryKey: ["queue", today] })
  });

  return (
    <div className="space-y-3 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">Queue / Token Management</h2>
      <ul className="space-y-2 text-sm">
        {(queue.data ?? []).map((a) => (
          <li key={a.id} className="rounded border p-2">
            <div>#{a.tokenNumber} {a.patientName} - Queue: {a.queueState}</div>
            <div className="mt-2 flex gap-2">
              {['Waiting','InProgress','Served','Skipped'].map(s => (
                <button key={s} className="rounded bg-slate-100 px-2 py-1 text-xs" onClick={() => update.mutate({ id: a.id, state: s })}>{s}</button>
              ))}
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
}
