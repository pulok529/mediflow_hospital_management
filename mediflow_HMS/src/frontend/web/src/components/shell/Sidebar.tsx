const navItems = [
  { label: "Dashboard", href: "/dashboard" },
  { label: "AI Assistant", href: "/ai/assistant" },
  { label: "Role Dashboards", href: "/reporting/roles" },
  { label: "Report Center", href: "/reporting/center" },
  { label: "Audit Explorer", href: "/reporting/audit" }
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
