import { useQuery } from "@tanstack/react-query";
import { reportingApi } from "@/lib/api";

export function AuditLogExplorerPage() {
  const logs = useQuery({ queryKey: ["audit-logs"], queryFn: reportingApi.auditLogs });
  return (
    <div className="rounded-xl bg-white p-6 shadow-sm">
      <h2 className="mb-3 text-xl font-bold">Audit Log Explorer</h2>
      <ul className="space-y-2 text-xs">
        {(logs.data ?? []).map((l: any) => (
          <li key={l.id} className="rounded border p-2">
            <p><strong>{l.action}</strong> | {l.entityType}:{l.entityId}</p>
            <p>{l.atUtc} | {l.details}</p>
          </li>
        ))}
      </ul>
    </div>
  );
}
