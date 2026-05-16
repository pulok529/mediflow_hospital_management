import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";

export type LiveNotification = {
  channel: string;
  message: string;
  atUtc: string;
  payload?: unknown;
};

type Listener = (n: LiveNotification) => void;
const listeners = new Set<Listener>();

const connection = new HubConnectionBuilder()
  .withUrl("http://localhost:8080/hubs/notifications")
  .withAutomaticReconnect()
  .configureLogging(LogLevel.Warning)
  .build();

connection.on("notification", (n: LiveNotification) => {
  listeners.forEach((cb) => cb(n));
});

export async function startNotifications() {
  if (connection.state === "Disconnected") {
    await connection.start();
  }
}

export function subscribeNotifications(listener: Listener) {
  listeners.add(listener);
  return () => listeners.delete(listener);
}
