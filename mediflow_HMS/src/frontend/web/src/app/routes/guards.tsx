import { Navigate, Outlet } from "react-router-dom";

const isAuthenticated = () => localStorage.getItem("mf_token") !== null;

export function AuthGuard() {
  return isAuthenticated() ? <Outlet /> : <Navigate to="/auth/login" replace />;
}

export function GuestGuard() {
  return isAuthenticated() ? <Navigate to="/dashboard" replace /> : <Outlet />;
}
