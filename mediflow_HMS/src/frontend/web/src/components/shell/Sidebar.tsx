const navItems = [
  { label: "Dashboard", href: "/dashboard" },
  { label: "Billing Dashboard", href: "/billing" },
  { label: "Running Bill", href: "/billing/running" },
  { label: "Payment Entry", href: "/billing/payments" },
  { label: "Refunds", href: "/billing/refunds" },
  { label: "Discount Queue", href: "/billing/discounts" },
  { label: "Discharge Checklist", href: "/billing/discharge-checklist" },
  { label: "Final Clearance", href: "/billing/final-clearance" }
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
