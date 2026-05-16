import { Navigate, Outlet } from "react-router-dom";
import { getRoles, isTokenExpired } from "@/lib/auth";

const hasToken = () => {
  const token = localStorage.getItem("mf_token");
  return !!token && !isTokenExpired(token);
};

export function AuthGuard() {
  return hasToken() ? <Outlet /> : <Navigate to="/auth/login" replace />;
}

export function GuestGuard() {
  return hasToken() ? <Navigate to="/dashboard" replace /> : <Outlet />;
}

export function RoleGuard({ roles }: { roles: string[] }) {
  const token = localStorage.getItem("mf_token");
  const userRoles = getRoles(token);
  const ok = userRoles.some((r) => roles.includes(r));
  return ok ? <Outlet /> : <Navigate to="/dashboard" replace />;
}
