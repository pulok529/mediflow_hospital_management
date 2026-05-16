import { createBrowserRouter } from "react-router-dom";
import { AuthLayout } from "@/app/layouts/AuthLayout";
import { DashboardLayout } from "@/app/layouts/DashboardLayout";
import { AuthGuard, GuestGuard } from "@/app/routes/guards";
import { DashboardPage } from "@/pages/DashboardPage";
import { LoginPage } from "@/pages/LoginPage";

export const router = createBrowserRouter([
  {
    element: <GuestGuard />,
    children: [
      {
        path: "/auth",
        element: <AuthLayout />,
        children: [{ path: "login", element: <LoginPage /> }]
      }
    ]
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
      }
    ]
  }
]);
