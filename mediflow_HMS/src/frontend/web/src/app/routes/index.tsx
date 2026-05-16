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
import { DoctorDashboardPage } from "@/pages/doctor/DoctorDashboardPage";
import { WaitingPatientsPage } from "@/pages/doctor/WaitingPatientsPage";
import { ConsultationPage } from "@/pages/doctor/ConsultationPage";
import { HistoryTimelinePage } from "@/pages/doctor/HistoryTimelinePage";
import { AdmissionDeskWizardPage } from "@/pages/admission/AdmissionDeskWizardPage";
import { BedConfigurationPage } from "@/pages/admission/BedConfigurationPage";
import { BedBoardPage } from "@/pages/admission/BedBoardPage";
import { FloorReceptionDashboardPage } from "@/pages/admission/FloorReceptionDashboardPage";
import { NurseStationDashboardPage } from "@/pages/nursing/NurseStationDashboardPage";
import { AssignedPatientsPage } from "@/pages/nursing/AssignedPatientsPage";
import { MedicationDueListPage } from "@/pages/nursing/MedicationDueListPage";
import { VitalsRecordingPage } from "@/pages/nursing/VitalsRecordingPage";
import { NursingNoteEntryPage } from "@/pages/nursing/NursingNoteEntryPage";
import { ShiftHandoverPage } from "@/pages/nursing/ShiftHandoverPage";
import { TransferDischargePreparationPage } from "@/pages/nursing/TransferDischargePreparationPage";

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
      },
      {
        element: <RoleGuard roles={["SuperAdmin", "Admin", "Doctor"]} />,
        children: [
          { path: "/doctor", element: <DoctorDashboardPage /> },
          { path: "/doctor/waiting", element: <WaitingPatientsPage /> },
          { path: "/doctor/consultation", element: <ConsultationPage /> },
          { path: "/doctor/history", element: <HistoryTimelinePage /> }
        ]
      },
      {
        element: <RoleGuard roles={["SuperAdmin", "Admin", "AdmissionOfficer", "FloorReceptionist"]} />,
        children: [
          { path: "/admission/wizard", element: <AdmissionDeskWizardPage /> },
          { path: "/admission/config", element: <BedConfigurationPage /> },
          { path: "/admission/bed-board", element: <BedBoardPage /> },
          { path: "/admission/floor-reception", element: <FloorReceptionDashboardPage /> }
        ]
      },
      {
        element: <RoleGuard roles={["SuperAdmin", "Admin", "Nurse", "FloorReceptionist"]} />,
        children: [
          { path: "/nursing", element: <NurseStationDashboardPage /> },
          { path: "/nursing/assigned", element: <AssignedPatientsPage /> },
          { path: "/nursing/medications", element: <MedicationDueListPage /> },
          { path: "/nursing/vitals", element: <VitalsRecordingPage /> },
          { path: "/nursing/notes", element: <NursingNoteEntryPage /> },
          { path: "/nursing/handover", element: <ShiftHandoverPage /> },
          { path: "/nursing/transfer-discharge", element: <TransferDischargePreparationPage /> }
        ]
      }
    ]
  }
]);
