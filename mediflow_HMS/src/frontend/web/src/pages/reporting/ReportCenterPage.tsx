import { useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { reportingApi } from "@/lib/api";

function downloadCsv(name: string, rows: any[]) {
  if (!rows.length) return;
  const headers = Object.keys(rows[0]);
  const lines = [headers.join(","), ...rows.map((r) => headers.map((h) => JSON.stringify(r[h] ?? "")).join(","))];
  const blob = new Blob([lines.join("\n")], { type: "text/csv" });
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url; a.download = `${name}.csv`; a.click(); URL.revokeObjectURL(url);
}

export function ReportCenterPage() {
  const [filter, setFilter] = useState({ fromDate: "", toDate: "", department: "", role: "", search: "" });
  const dataset = useMutation({ mutationFn: () => reportingApi.datasets({ ...filter, fromDate: filter.fromDate || null, toDate: filter.toDate || null }) });
  const printable = useMutation({ mutationFn: () => reportingApi.printable({ title: "Mediflow Report", ...filter, fromDate: filter.fromDate || null, toDate: filter.toDate || null }), onSuccess: (blob) => {
    const url = URL.createObjectURL(blob); const a = document.createElement("a"); a.href = url; a.download = "report.pdf"; a.click(); URL.revokeObjectURL(url);
  }});

  return (
    <div className="rounded-xl bg-white p-6 shadow-sm space-y-3">
      <h2 className="text-xl font-bold">Report Center</h2>
      <div className="grid gap-2 md:grid-cols-5">
        {Object.keys(filter).map((k) => <input key={k} className="rounded border px-3 py-2" placeholder={k} value={(filter as any)[k]} onChange={(e)=>setFilter({...filter,[k]:e.target.value})} />)}
      </div>
      <div className="flex gap-2">
        <button className="rounded bg-slate-900 px-4 py-2 text-white" onClick={() => dataset.mutate()}>Load Datasets</button>
        <button className="rounded bg-green-700 px-4 py-2 text-white" onClick={() => printable.mutate()}>Export PDF</button>
      </div>
      <div className="space-y-3">
        {(dataset.data ?? []).map((d: any) => (
          <div key={d.name} className="rounded border p-3 text-sm">
            <div className="flex justify-between"><strong>{d.name}</strong><button className="rounded bg-slate-100 px-2 py-1" onClick={() => downloadCsv(d.name, d.rows)}>Export CSV</button></div>
            <p>Rows: {d.rows?.length ?? 0}</p>
          </div>
        ))}
      </div>
    </div>
  );
}
