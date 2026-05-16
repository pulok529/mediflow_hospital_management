import { useState } from "react";
import type { FormEvent } from "react";
import { api } from "@/lib/api";

export function LoginPage() {
  const [email, setEmail] = useState("superadmin@mediflow.local");
  const [password, setPassword] = useState("SuperAdmin123!");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError("");
    try {
      const tokens = await api.login(email, password);
      localStorage.setItem("mf_token", tokens.accessToken);
      localStorage.setItem("mf_refresh", tokens.refreshToken);
      window.location.href = "/dashboard";
    } catch (err) {
      setError(err instanceof Error ? err.message : "Login failed");
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={onSubmit} className="space-y-4">
      <h2 className="text-2xl font-bold">Sign in to Mediflow</h2>
      <p className="text-sm text-slate-600">Use seeded admin credentials to access RBAC setup.</p>
      <input className="w-full rounded-lg border px-3 py-2" value={email} onChange={(e) => setEmail(e.target.value)} />
      <input className="w-full rounded-lg border px-3 py-2" type="password" value={password} onChange={(e) => setPassword(e.target.value)} />
      {error ? <p className="text-sm text-red-600">{error}</p> : null}
      <button disabled={loading} className="w-full rounded-lg bg-slate-900 px-4 py-2 font-medium text-white hover:bg-slate-700">
        {loading ? "Signing in..." : "Sign in"}
      </button>
    </form>
  );
}
