import { createBrowserRouter } from "react-router-dom";
import { AuthLayout } from "@/app/layouts/AuthLayout";
import { DashboardLayout } from "@/app/layouts/DashboardLayout";
import { AuthGuard, GuestGuard, RoleGuard } from "@/app/routes/guards";
import { DashboardPage } from "@/pages/DashboardPage";
import { LoginPage } from "@/pages/LoginPage";
import { AiAssistantPage } from "@/pages/ai/AiAssistantPage";
import { RoleDashboardsPage } from "@/pages/reporting/RoleDashboardsPage";
import { ReportCenterPage } from "@/pages/reporting/ReportCenterPage";
import { AuditLogExplorerPage } from "@/pages/reporting/AuditLogExplorerPage";

export const router = createBrowserRouter([
  { element: <GuestGuard />, children: [{ path: "/auth", element: <AuthLayout />, children: [{ path: "login", element: <LoginPage /> }] }] },
  {
    element: <AuthGuard />,
    children: [
      {
        path: "/",
        element: <DashboardLayout />,
        children: [
          { index: true, element: <DashboardPage /> },
          { path: "dashboard", element: <DashboardPage /> },
          { path: "ai/assistant", element: <AiAssistantPage /> }
        ]
      },
      {
        element: <RoleGuard roles={["SuperAdmin", "Admin"]} />,
        children: [
          { path: "/reporting/roles", element: <RoleDashboardsPage /> },
          { path: "/reporting/center", element: <ReportCenterPage /> },
          { path: "/reporting/audit", element: <AuditLogExplorerPage /> }
        ]
      }
    ]
  }
]);
