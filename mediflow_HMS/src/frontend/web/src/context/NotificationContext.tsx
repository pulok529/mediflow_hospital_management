import { createContext, useContext, useEffect, useMemo, useState } from "react";
import { startNotifications, subscribeNotifications } from "@/lib/realtime";
import type { LiveNotification } from "@/lib/realtime";

type Ctx = { notifications: LiveNotification[]; unread: number; clear: () => void };
const NotificationContext = createContext<Ctx | null>(null);

export function NotificationProvider({ children }: { children: React.ReactNode }) {
  const [notifications, setNotifications] = useState<LiveNotification[]>([]);
  const [unread, setUnread] = useState(0);

  useEffect(() => {
    startNotifications().catch(() => undefined);
    const unsubscribe = subscribeNotifications((n) => {
      setNotifications((prev) => [n, ...prev].slice(0, 100));
      setUnread((u) => u + 1);
    });

    return () => {
      unsubscribe();
    };
  }, []);

  const value = useMemo(() => ({ notifications, unread, clear: () => setUnread(0) }), [notifications, unread]);
  return <NotificationContext.Provider value={value}>{children}</NotificationContext.Provider>;
}

export function useNotifications() {
  const ctx = useContext(NotificationContext);
  if (!ctx) throw new Error("useNotifications must be used within NotificationProvider");
  return ctx;
}
