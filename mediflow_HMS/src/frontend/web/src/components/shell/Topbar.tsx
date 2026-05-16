import { NotificationDrawer } from "@/components/notifications/NotificationDrawer";

export function Topbar() {
  return (
    <header className="flex h-16 items-center justify-between border-b bg-white px-6">
      <h1 className="text-lg font-semibold text-slate-800">Hospital Management System</h1>
      <NotificationDrawer />
    </header>
  );
}
