import { createBrowserRouter } from "react-router-dom";
import { AuthLayout } from "@/app/layouts/AuthLayout";
import { DashboardLayout } from "@/app/layouts/DashboardLayout";
import { AuthGuard, GuestGuard, RoleGuard } from "@/app/routes/guards";
import { DashboardPage } from "@/pages/DashboardPage";
import { LoginPage } from "@/pages/LoginPage";
import { UserManagementPage } from "@/pages/admin/UserManagementPage";
import { RoleManagementPage } from "@/pages/admin/RoleManagementPage";
import { PermissionAssignmentPage } from "@/pages/admin/PermissionAssignmentPage";

export const router = createBrowserRouter([
  {
    element: <GuestGuard />,
    children: [{ path: "/auth", element: <AuthLayout />, children: [{ path: "login", element: <LoginPage /> }] }]
  },
  {
    element: <AuthGuard />,
    children: [
      {
        path: "/",
        element: <DashboardLayout />,
        children: [
          { index: true, element: <DashboardPage /> },
          { path: "dashboard", element: <DashboardPage /> }
        ]
      },
      {
        element: <RoleGuard roles={["SuperAdmin", "Admin"]} />,
        children: [
          { path: "/admin/users", element: <UserManagementPage /> },
          { path: "/admin/roles", element: <RoleManagementPage /> },
          { path: "/admin/permissions", element: <PermissionAssignmentPage /> }
        ]
      }
    ]
  }
]);
