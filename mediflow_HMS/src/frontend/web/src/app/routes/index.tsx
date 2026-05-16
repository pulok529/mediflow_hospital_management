import { createBrowserRouter } from "react-router-dom";
import { AuthLayout } from "@/app/layouts/AuthLayout";
import { DashboardLayout } from "@/app/layouts/DashboardLayout";
import { AuthGuard, GuestGuard, RoleGuard } from "@/app/routes/guards";
import { DashboardPage } from "@/pages/DashboardPage";
import { LoginPage } from "@/pages/LoginPage";
import { BillingDashboardPage } from "@/pages/billing/BillingDashboardPage";
import { RunningBillViewPage } from "@/pages/billing/RunningBillViewPage";
import { PaymentEntryPage } from "@/pages/billing/PaymentEntryPage";
import { RefundScreenPage } from "@/pages/billing/RefundScreenPage";
import { DiscountApprovalQueuePage } from "@/pages/billing/DiscountApprovalQueuePage";
import { DischargeChecklistPage } from "@/pages/billing/DischargeChecklistPage";
import { FinalBillClearancePage } from "@/pages/billing/FinalBillClearancePage";

export const router = createBrowserRouter([
  { element: <GuestGuard />, children: [{ path: "/auth", element: <AuthLayout />, children: [{ path: "login", element: <LoginPage /> }] }] },
  {
    element: <AuthGuard />,
    children: [
      { path: "/", element: <DashboardLayout />, children: [{ index: true, element: <DashboardPage /> }, { path: "dashboard", element: <DashboardPage /> }] },
      {
        element: <RoleGuard roles={["SuperAdmin", "Admin", "BillingOfficer"]} />,
        children: [
          { path: "/billing", element: <BillingDashboardPage /> },
          { path: "/billing/running", element: <RunningBillViewPage /> },
          { path: "/billing/payments", element: <PaymentEntryPage /> },
          { path: "/billing/refunds", element: <RefundScreenPage /> },
          { path: "/billing/discounts", element: <DiscountApprovalQueuePage /> },
          { path: "/billing/discharge-checklist", element: <DischargeChecklistPage /> },
          { path: "/billing/final-clearance", element: <FinalBillClearancePage /> }
        ]
      }
    ]
  }
]);
