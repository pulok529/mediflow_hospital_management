import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";

export function UserManagementPage() {
  const users = useQuery({ queryKey: ["users"], queryFn: api.users.list });

  return (
    <div className="space-y-4 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">User Management</h2>
      <p className="text-sm text-slate-500">Enterprise user administration with role mapping.</p>
      <div className="overflow-auto">
        <table className="min-w-full text-sm">
          <thead>
            <tr className="border-b text-left">
              <th className="py-2">Email</th>
              <th>Status</th>
              <th>Roles</th>
            </tr>
          </thead>
          <tbody>
            {(users.data ?? []).map((u) => (
              <tr key={u.id} className="border-b">
                <td className="py-2">{u.email}</td>
                <td>{u.isActive ? "Active" : "Disabled"}</td>
                <td>{(u.roles ?? []).join(", ")}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
