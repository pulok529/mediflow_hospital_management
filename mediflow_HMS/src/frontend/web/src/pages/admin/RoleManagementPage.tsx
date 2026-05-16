import { useState } from "react";
import type { FormEvent } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { api } from "@/lib/api";

export function RoleManagementPage() {
  const qc = useQueryClient();
  const [name, setName] = useState("");
  const roles = useQuery({ queryKey: ["roles"], queryFn: api.roles.list });
  const createRole = useMutation({
    mutationFn: () => api.roles.create({ name }),
    onSuccess: () => {
      setName("");
      qc.invalidateQueries({ queryKey: ["roles"] });
    }
  });

  const onSubmit = (e: FormEvent) => {
    e.preventDefault();
    if (!name.trim()) return;
    createRole.mutate();
  };

  return (
    <div className="space-y-4 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">Role Management</h2>
      <form onSubmit={onSubmit} className="flex gap-2">
        <input value={name} onChange={(e) => setName(e.target.value)} className="rounded-lg border px-3 py-2" placeholder="New role" />
        <button className="rounded-lg bg-slate-900 px-4 py-2 text-white">Create</button>
      </form>
      <ul className="space-y-2 text-sm">
        {(roles.data ?? []).map((r) => (
          <li key={r.id} className="rounded border p-2">
            <p className="font-semibold">{r.name}</p>
            <p className="text-slate-500">Permissions: {(r.permissions ?? []).join(", ") || "None"}</p>
          </li>
        ))}
      </ul>
    </div>
  );
}
