import { useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { admissionApi } from "@/lib/api";

export function BedConfigurationPage() {
  const qc = useQueryClient();
  const h = useQuery({ queryKey: ["hierarchy"], queryFn: admissionApi.config.hierarchy });

  const [building, setBuilding] = useState("");
  const [floor, setFloor] = useState({ buildingId: "", name: "" });
  const [ward, setWard] = useState({ floorId: "", name: "" });
  const [room, setRoom] = useState({ wardId: "", name: "" });
  const [bed, setBed] = useState({ roomId: "", bedNumber: "" });

  const mk = (fn: () => Promise<any>) => useMutation({ mutationFn: fn, onSuccess: () => qc.invalidateQueries({ queryKey: ["hierarchy"] }) });
  const createBuilding = mk(() => admissionApi.config.createBuilding({ name: building }));
  const createFloor = mk(() => admissionApi.config.createFloor(floor));
  const createWard = mk(() => admissionApi.config.createWard(ward));
  const createRoom = mk(() => admissionApi.config.createRoom(room));
  const createBed = mk(() => admissionApi.config.createBed(bed));

  return (
    <div className="space-y-4 rounded-xl bg-white p-6 shadow-sm">
      <h2 className="text-xl font-bold">Building / Floor / Room / Bed Configuration</h2>
      <div className="grid gap-2 md:grid-cols-2">
        <input className="rounded border px-3 py-2" placeholder="Building name" value={building} onChange={(e) => setBuilding(e.target.value)} />
        <button className="rounded bg-slate-900 px-3 py-2 text-white" onClick={() => createBuilding.mutate()}>Add Building</button>

        <input className="rounded border px-3 py-2" placeholder="Floor name" value={floor.name} onChange={(e) => setFloor({ ...floor, name: e.target.value })} />
        <select className="rounded border px-3 py-2" value={floor.buildingId} onChange={(e) => setFloor({ ...floor, buildingId: e.target.value })}>
          <option value="">Select building</option>
          {(h.data?.buildings ?? []).map((b: any) => <option key={b.id} value={b.id}>{b.name}</option>)}
        </select>
        <button className="rounded bg-slate-900 px-3 py-2 text-white" onClick={() => createFloor.mutate()}>Add Floor</button>

        <input className="rounded border px-3 py-2" placeholder="Ward name" value={ward.name} onChange={(e) => setWard({ ...ward, name: e.target.value })} />
        <select className="rounded border px-3 py-2" value={ward.floorId} onChange={(e) => setWard({ ...ward, floorId: e.target.value })}>
          <option value="">Select floor</option>
          {(h.data?.floors ?? []).map((f: any) => <option key={f.id} value={f.id}>{f.name}</option>)}
        </select>
        <button className="rounded bg-slate-900 px-3 py-2 text-white" onClick={() => createWard.mutate()}>Add Ward</button>

        <input className="rounded border px-3 py-2" placeholder="Room name" value={room.name} onChange={(e) => setRoom({ ...room, name: e.target.value })} />
        <select className="rounded border px-3 py-2" value={room.wardId} onChange={(e) => setRoom({ ...room, wardId: e.target.value })}>
          <option value="">Select ward</option>
          {(h.data?.wards ?? []).map((w: any) => <option key={w.id} value={w.id}>{w.name}</option>)}
        </select>
        <button className="rounded bg-slate-900 px-3 py-2 text-white" onClick={() => createRoom.mutate()}>Add Room</button>

        <input className="rounded border px-3 py-2" placeholder="Bed number" value={bed.bedNumber} onChange={(e) => setBed({ ...bed, bedNumber: e.target.value })} />
        <select className="rounded border px-3 py-2" value={bed.roomId} onChange={(e) => setBed({ ...bed, roomId: e.target.value })}>
          <option value="">Select room</option>
          {(h.data?.rooms ?? []).map((r: any) => <option key={r.id} value={r.id}>{r.name}</option>)}
        </select>
        <button className="rounded bg-slate-900 px-3 py-2 text-white" onClick={() => createBed.mutate()}>Add Bed</button>
      </div>
    </div>
  );
}
