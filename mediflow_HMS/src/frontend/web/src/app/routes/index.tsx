import { createBrowserRouter } from "react-router-dom";
import { AuthLayout } from "@/app/layouts/AuthLayout";
import { DashboardLayout } from "@/app/layouts/DashboardLayout";
import { AuthGuard, GuestGuard, RoleGuard } from "@/app/routes/guards";
import { DashboardPage } from "@/pages/DashboardPage";
import { LoginPage } from "@/pages/LoginPage";
import { BedBoardPage } from "@/pages/admission/BedBoardPage";
import { BedConfigurationPage } from "@/pages/admission/BedConfigurationPage";
import { AdmissionDeskWizardPage } from "@/pages/admission/AdmissionDeskWizardPage";
import { FloorReceptionDashboardPage } from "@/pages/admission/FloorReceptionDashboardPage";
import { PermissionAssignmentPage } from "@/pages/admin/PermissionAssignmentPage";
import { RoleManagementPage } from "@/pages/admin/RoleManagementPage";
import { UserManagementPage } from "@/pages/admin/UserManagementPage";
import { AiAssistantPage } from "@/pages/ai/AiAssistantPage";
import { BillingDashboardPage } from "@/pages/billing/BillingDashboardPage";
import { DiscountApprovalQueuePage } from "@/pages/billing/DiscountApprovalQueuePage";
import { DischargeChecklistPage } from "@/pages/billing/DischargeChecklistPage";
import { FinalBillClearancePage } from "@/pages/billing/FinalBillClearancePage";
import { PaymentEntryPage } from "@/pages/billing/PaymentEntryPage";
import { RefundScreenPage } from "@/pages/billing/RefundScreenPage";
import { RunningBillViewPage } from "@/pages/billing/RunningBillViewPage";
import { ConsultationPage } from "@/pages/doctor/ConsultationPage";
import { DoctorDashboardPage } from "@/pages/doctor/DoctorDashboardPage";
import { HistoryTimelinePage } from "@/pages/doctor/HistoryTimelinePage";
import { WaitingPatientsPage } from "@/pages/doctor/WaitingPatientsPage";
import { ImagingScheduleReportPage } from "@/pages/diagnostics/ImagingScheduleReportPage";
import { LabDashboardPage } from "@/pages/diagnostics/LabDashboardPage";
import { LabResultApprovalPage } from "@/pages/diagnostics/LabResultApprovalPage";
import { RadiologyDashboardPage } from "@/pages/diagnostics/RadiologyDashboardPage";
import { SampleCollectionPage } from "@/pages/diagnostics/SampleCollectionPage";
import { AssignedPatientsPage } from "@/pages/nursing/AssignedPatientsPage";
import { MedicationDueListPage } from "@/pages/nursing/MedicationDueListPage";
import { NurseStationDashboardPage } from "@/pages/nursing/NurseStationDashboardPage";
import { NursingNoteEntryPage } from "@/pages/nursing/NursingNoteEntryPage";
import { ShiftHandoverPage } from "@/pages/nursing/ShiftHandoverPage";
import { TransferDischargePreparationPage } from "@/pages/nursing/TransferDischargePreparationPage";
import { VitalsRecordingPage } from "@/pages/nursing/VitalsRecordingPage";
import { InventoryDashboardPage } from "@/pages/pharmacy/InventoryDashboardPage";
import { PharmacyDashboardPage } from "@/pages/pharmacy/PharmacyDashboardPage";
import { PrescriptionDispensingPage } from "@/pages/pharmacy/PrescriptionDispensingPage";
import { PurchaseStockIssuePage } from "@/pages/pharmacy/PurchaseStockIssuePage";
import { StockBatchViewPage } from "@/pages/pharmacy/StockBatchViewPage";
import { AppointmentBookingPage } from "@/pages/reception/AppointmentBookingPage";
import { DailyAppointmentListPage } from "@/pages/reception/DailyAppointmentListPage";
import { PatientRegistrationPage } from "@/pages/reception/PatientRegistrationPage";
import { PatientSearchPage } from "@/pages/reception/PatientSearchPage";
import { QueueManagementPage } from "@/pages/reception/QueueManagementPage";
import { ReceptionDashboardPage } from "@/pages/reception/ReceptionDashboardPage";
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
          {
            element: <RoleGuard roles={["SuperAdmin", "Admin", "Doctor", "BillingOfficer", "Pharmacist", "LabTech", "Radiologist", "Nurse"]} />,
            children: [{ path: "ai/assistant", element: <AiAssistantPage /> }]
          },
          {
            element: <RoleGuard roles={["SuperAdmin", "Admin"]} />,
            children: [
              { path: "admin/users", element: <UserManagementPage /> },
              { path: "admin/roles", element: <RoleManagementPage /> },
              { path: "admin/permissions", element: <PermissionAssignmentPage /> },
              { path: "reporting/roles", element: <RoleDashboardsPage /> },
              { path: "reporting/center", element: <ReportCenterPage /> },
              { path: "reporting/audit", element: <AuditLogExplorerPage /> }
            ]
          },
          {
            element: <RoleGuard roles={["SuperAdmin", "Admin", "Receptionist"]} />,
            children: [
              { path: "reception", element: <ReceptionDashboardPage /> },
              { path: "reception/patients/new", element: <PatientRegistrationPage /> },
              { path: "reception/patients/search", element: <PatientSearchPage /> },
              { path: "reception/appointments/new", element: <AppointmentBookingPage /> },
              { path: "reception/appointments/daily", element: <DailyAppointmentListPage /> },
              { path: "reception/queue", element: <QueueManagementPage /> }
            ]
          },
          {
            element: <RoleGuard roles={["SuperAdmin", "Admin", "Doctor"]} />,
            children: [
              { path: "doctor", element: <DoctorDashboardPage /> },
              { path: "doctor/waiting", element: <WaitingPatientsPage /> },
              { path: "doctor/consultation", element: <ConsultationPage /> },
              { path: "doctor/history", element: <HistoryTimelinePage /> }
            ]
          },
          {
            element: <RoleGuard roles={["SuperAdmin", "Admin", "AdmissionOfficer", "FloorReceptionist"]} />,
            children: [
              { path: "admission/config", element: <BedConfigurationPage /> },
              { path: "admission/desk", element: <AdmissionDeskWizardPage /> },
              { path: "admission/bed-board", element: <BedBoardPage /> },
              { path: "admission/floor", element: <FloorReceptionDashboardPage /> }
            ]
          },
          {
            element: <RoleGuard roles={["SuperAdmin", "Admin", "Nurse", "FloorReceptionist"]} />,
            children: [
              { path: "nursing", element: <NurseStationDashboardPage /> },
              { path: "nursing/patients", element: <AssignedPatientsPage /> },
              { path: "nursing/notes", element: <NursingNoteEntryPage /> },
              { path: "nursing/medications", element: <MedicationDueListPage /> },
              { path: "nursing/vitals", element: <VitalsRecordingPage /> },
              { path: "nursing/handover", element: <ShiftHandoverPage /> },
              { path: "nursing/discharge-prep", element: <TransferDischargePreparationPage /> }
            ]
          },
          {
            element: <RoleGuard roles={["SuperAdmin", "Admin", "LabTech", "Radiologist", "Doctor"]} />,
            children: [
              { path: "diagnostics/lab", element: <LabDashboardPage /> },
              { path: "diagnostics/lab/samples", element: <SampleCollectionPage /> },
              { path: "diagnostics/lab/results", element: <LabResultApprovalPage /> },
              { path: "diagnostics/radiology", element: <RadiologyDashboardPage /> },
              { path: "diagnostics/radiology/reports", element: <ImagingScheduleReportPage /> }
            ]
          },
          {
            element: <RoleGuard roles={["SuperAdmin", "Admin", "Pharmacist"]} />,
            children: [
              { path: "pharmacy", element: <PharmacyDashboardPage /> },
              { path: "pharmacy/inventory", element: <InventoryDashboardPage /> },
              { path: "pharmacy/dispense", element: <PrescriptionDispensingPage /> },
              { path: "pharmacy/purchase", element: <PurchaseStockIssuePage /> },
              { path: "pharmacy/batches", element: <StockBatchViewPage /> }
            ]
          },
          {
            element: <RoleGuard roles={["SuperAdmin", "Admin", "BillingOfficer"]} />,
            children: [
              { path: "billing", element: <BillingDashboardPage /> },
              { path: "billing/running", element: <RunningBillViewPage /> },
              { path: "billing/payments", element: <PaymentEntryPage /> },
              { path: "billing/discounts", element: <DiscountApprovalQueuePage /> },
              { path: "billing/refunds", element: <RefundScreenPage /> },
              { path: "billing/final-clearance", element: <FinalBillClearancePage /> },
              { path: "billing/discharge", element: <DischargeChecklistPage /> }
            ]
          }
        ]
      }
    ]
  }
]);
