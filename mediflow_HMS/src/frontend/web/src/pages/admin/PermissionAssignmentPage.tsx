import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api";

export function PermissionAssignmentPage() {
  const qc = useQueryClient();
  const roles = useQuery({ queryKey: ["roles"], queryFn: api.roles.list });
  const permissions = useQuery({ queryKey: ["permissions"], queryFn: api.permissions.list });

  const assign = useMutation({
    mutationFn: ({ roleId, permission }: { roleId: string; permission: string }) => {
      const role = (roles.data ?? []).find((r) => r.id === roleId);
      const existing = new Set<string>(role?.permissions ?? []);
      existing.add(permission);
      return api.roles.assignPermissions(roleId, Array.from(existing));
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: ["roles"] })
  });

  return (
    <div className="space-y-4 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">Permission Assignment</h2>
      <div className="grid gap-3 md:grid-cols-2">
        {(roles.data ?? []).map((r) => (
          <div key={r.id} className="rounded border p-3">
            <p className="font-semibold">{r.name}</p>
            <p className="mb-2 text-xs text-slate-500">Current: {(r.permissions ?? []).join(", ") || "None"}</p>
            <div className="flex flex-wrap gap-2">
              {(permissions.data ?? []).map((p) => (
                <button
                  key={p}
                  onClick={() => assign.mutate({ roleId: r.id, permission: p })}
                  className="rounded bg-slate-100 px-2 py-1 text-xs hover:bg-slate-200"
                >
                  + {p}
                </button>
              ))}
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
