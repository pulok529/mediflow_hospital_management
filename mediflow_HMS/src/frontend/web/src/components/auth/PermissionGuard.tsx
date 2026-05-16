import type { ReactElement } from "react";
import { Navigate } from "react-router-dom";
import { getPermissions } from "@/lib/auth";

type Props = { permission: string; children: ReactElement };

export function PermissionGuard({ permission, children }: Props) {
  const token = localStorage.getItem("mf_token");
  const permissions = getPermissions(token);
  if (!permissions.includes(permission)) {
    return <Navigate to="/dashboard" replace />;
  }
  return children;
}
