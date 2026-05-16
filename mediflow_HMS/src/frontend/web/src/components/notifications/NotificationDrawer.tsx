import { useState } from "react";
import { useNotifications } from "@/context/NotificationContext";

export function NotificationDrawer() {
  const { notifications, unread, clear } = useNotifications();
  const [open, setOpen] = useState(false);

  return (
    <div className="relative">
      <button
        className="rounded border px-3 py-1 text-sm"
        onClick={() => {
          setOpen((o) => !o);
          clear();
        }}
      >
        Notifications ({unread})
      </button>
      {open ? (
        <div className="absolute right-0 mt-2 w-96 max-h-80 overflow-auto rounded-lg border bg-white p-3 shadow">
          <p className="mb-2 text-sm font-semibold">Notification Center</p>
          <ul className="space-y-2 text-xs">
            {notifications.map((n, i) => (
              <li key={i} className="rounded border p-2">
                <p><strong>{n.channel}</strong> - {n.message}</p>
                <p className="text-slate-500">{n.atUtc}</p>
              </li>
            ))}
          </ul>
        </div>
      ) : null}
    </div>
  );
}
