import { useQuery } from "@tanstack/react-query";
import { billingApi } from "@/lib/api";

export function BillingDashboardPage() {
  const data = useQuery({ queryKey: ["billing-dashboard"], queryFn: billingApi.dashboard });
  return <div className="rounded-xl bg-white p-6 shadow-sm"><h2 className="text-xl font-bold">Billing Dashboard</h2><p className="text-sm">Bills: {(data.data?.bills ?? []).length}</p><p className="text-sm">Discount queue: {(data.data?.discounts ?? []).length}</p></div>;
}
