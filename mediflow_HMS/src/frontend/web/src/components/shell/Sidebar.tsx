const navItems = [
  { label: "Dashboard", href: "/dashboard" },
  { label: "User Management", href: "/admin/users" },
  { label: "Role Management", href: "/admin/roles" },
  { label: "Permission Assignment", href: "/admin/permissions" },
  { label: "Reception Dashboard", href: "/reception" },
  { label: "Patient Registration", href: "/reception/register" },
  { label: "Patient Search", href: "/reception/search" },
  { label: "Appointment Booking", href: "/reception/book" },
  { label: "Daily Appointments", href: "/reception/daily" },
  { label: "Queue Management", href: "/reception/queue" },
  { label: "Doctor Dashboard", href: "/doctor" },
  { label: "Waiting Patients", href: "/doctor/waiting" },
  { label: "Consultation", href: "/doctor/consultation" },
  { label: "History Timeline", href: "/doctor/history" },
  { label: "Admission Wizard", href: "/admission/wizard" },
  { label: "Bed Configuration", href: "/admission/config" },
  { label: "Live Bed Board", href: "/admission/bed-board" },
  { label: "Floor Reception", href: "/admission/floor-reception" }
];

export function Sidebar() {
  return (
    <aside className="w-64 border-r bg-white p-4">
      <p className="mb-6 text-xl font-bold">Mediflow HMS</p>
      <nav className="space-y-2">
        {navItems.map((item) => (
          <a key={item.label} href={item.href} className="block rounded-lg px-3 py-2 text-sm text-slate-700 hover:bg-slate-100">
            {item.label}
          </a>
        ))}
      </nav>
    </aside>
  );
}
