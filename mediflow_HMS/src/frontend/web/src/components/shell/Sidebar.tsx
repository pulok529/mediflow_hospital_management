import { Link } from "react-router-dom";

const navGroups = [
  {
    label: "Core",
    items: [
      { label: "Dashboard", href: "/dashboard" },
      { label: "AI Assistant", href: "/ai/assistant" }
    ]
  },
  {
    label: "Reception",
    items: [
      { label: "Reception Dashboard", href: "/reception" },
      { label: "Patient Registration", href: "/reception/patients/new" },
      { label: "Patient Search", href: "/reception/patients/search" },
      { label: "Appointment Booking", href: "/reception/appointments/new" },
      { label: "Daily Appointments", href: "/reception/appointments/daily" },
      { label: "Queue", href: "/reception/queue" }
    ]
  },
  {
    label: "Clinical",
    items: [
      { label: "Doctor Dashboard", href: "/doctor" },
      { label: "Waiting Patients", href: "/doctor/waiting" },
      { label: "Consultation", href: "/doctor/consultation" },
      { label: "Patient History", href: "/doctor/history" },
      { label: "Nurse Station", href: "/nursing" },
      { label: "Nursing Notes", href: "/nursing/notes" },
      { label: "Vitals", href: "/nursing/vitals" }
    ]
  },
  {
    label: "Operations",
    items: [
      { label: "Admission Desk", href: "/admission/desk" },
      { label: "Bed Board", href: "/admission/bed-board" },
      { label: "Lab", href: "/diagnostics/lab" },
      { label: "Radiology", href: "/diagnostics/radiology" },
      { label: "Pharmacy", href: "/pharmacy" },
      { label: "Billing", href: "/billing" }
    ]
  },
  {
    label: "Admin",
    items: [
      { label: "Users", href: "/admin/users" },
      { label: "Roles", href: "/admin/roles" },
      { label: "Permissions", href: "/admin/permissions" },
      { label: "Report Center", href: "/reporting/center" },
      { label: "Audit Explorer", href: "/reporting/audit" }
    ]
  }
];

export function Sidebar() {
  return (
    <aside className="h-screen w-72 overflow-y-auto border-r bg-white p-4">
      <p className="mb-6 text-xl font-bold">Mediflow HMS</p>
      <nav className="space-y-5">
        {navGroups.map((group) => (
          <section key={group.label}>
            <p className="mb-2 px-3 text-xs font-semibold uppercase tracking-wide text-slate-500">{group.label}</p>
            <div className="space-y-1">
              {group.items.map((item) => (
                <Link key={item.label} to={item.href} className="block rounded-md px-3 py-2 text-sm text-slate-700 hover:bg-slate-100">
                  {item.label}
                </Link>
              ))}
            </div>
          </section>
        ))}
      </nav>
    </aside>
  );
}
