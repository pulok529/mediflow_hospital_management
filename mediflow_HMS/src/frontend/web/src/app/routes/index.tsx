import { createBrowserRouter } from "react-router-dom";
import { AuthLayout } from "@/app/layouts/AuthLayout";
import { DashboardLayout } from "@/app/layouts/DashboardLayout";
import { AuthGuard, GuestGuard, RoleGuard } from "@/app/routes/guards";
import { DashboardPage } from "@/pages/DashboardPage";
import { LoginPage } from "@/pages/LoginPage";
import { PharmacyDashboardPage } from "@/pages/pharmacy/PharmacyDashboardPage";
import { PrescriptionDispensingPage } from "@/pages/pharmacy/PrescriptionDispensingPage";
import { StockBatchViewPage } from "@/pages/pharmacy/StockBatchViewPage";
import { InventoryDashboardPage } from "@/pages/pharmacy/InventoryDashboardPage";
import { PurchaseStockIssuePage } from "@/pages/pharmacy/PurchaseStockIssuePage";

export const router = createBrowserRouter([
  { element: <GuestGuard />, children: [{ path: "/auth", element: <AuthLayout />, children: [{ path: "login", element: <LoginPage /> }] }] },
  {
    element: <AuthGuard />,
    children: [
      { path: "/", element: <DashboardLayout />, children: [{ index: true, element: <DashboardPage /> }, { path: "dashboard", element: <DashboardPage /> }] },
      {
        element: <RoleGuard roles={["SuperAdmin", "Admin", "Pharmacist"]} />,
        children: [
          { path: "/pharmacy", element: <PharmacyDashboardPage /> },
          { path: "/pharmacy/dispense", element: <PrescriptionDispensingPage /> },
          { path: "/pharmacy/stock", element: <StockBatchViewPage /> },
          { path: "/inventory", element: <InventoryDashboardPage /> },
          { path: "/inventory/purchase-issue", element: <PurchaseStockIssuePage /> }
        ]
      }
    ]
  }
]);
