import { createBrowserRouter } from "react-router-dom";
import { AuthLayout } from "@/app/layouts/AuthLayout";
import { DashboardLayout } from "@/app/layouts/DashboardLayout";
import { AuthGuard, GuestGuard, RoleGuard } from "@/app/routes/guards";
import { DashboardPage } from "@/pages/DashboardPage";
import { LoginPage } from "@/pages/LoginPage";
import { UserManagementPage } from "@/pages/admin/UserManagementPage";
import { RoleManagementPage } from "@/pages/admin/RoleManagementPage";
import { PermissionAssignmentPage } from "@/pages/admin/PermissionAssignmentPage";
import { ReceptionDashboardPage } from "@/pages/reception/ReceptionDashboardPage";
import { PatientRegistrationPage } from "@/pages/reception/PatientRegistrationPage";
import { PatientSearchPage } from "@/pages/reception/PatientSearchPage";
import { AppointmentBookingPage } from "@/pages/reception/AppointmentBookingPage";
import { DailyAppointmentListPage } from "@/pages/reception/DailyAppointmentListPage";
import { QueueManagementPage } from "@/pages/reception/QueueManagementPage";

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
      },
      {
        element: <RoleGuard roles={["SuperAdmin", "Admin", "Receptionist"]} />,
        children: [
          { path: "/reception", element: <ReceptionDashboardPage /> },
          { path: "/reception/register", element: <PatientRegistrationPage /> },
          { path: "/reception/search", element: <PatientSearchPage /> },
          { path: "/reception/book", element: <AppointmentBookingPage /> },
          { path: "/reception/daily", element: <DailyAppointmentListPage /> },
          { path: "/reception/queue", element: <QueueManagementPage /> }
        ]
      }
    ]
  }
]);
