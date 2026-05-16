const navItems = [
  { label: "Dashboard", href: "/dashboard" },
  { label: "Pharmacy Dashboard", href: "/pharmacy" },
  { label: "Prescription Dispense", href: "/pharmacy/dispense" },
  { label: "Stock / Batch", href: "/pharmacy/stock" },
  { label: "Inventory Dashboard", href: "/inventory" },
  { label: "Purchase & Issue", href: "/inventory/purchase-issue" }
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
