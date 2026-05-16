import { useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { nursingApi } from "@/lib/api";

export function NursingNoteEntryPage() {
  const [admissionId, setAdmissionId] = useState("");
  const [nurseName, setNurseName] = useState("Nurse");
  const [note, setNote] = useState("");
  const save = useMutation({ mutationFn: () => nursingApi.notes.add({ admissionId, nurseName, note, linkedDoctorRoundEncounterId: null }) });

  return (
    <div className="rounded-xl bg-white p-6 shadow-sm space-y-3">
      <h2 className="text-xl font-bold">Nursing Note Entry</h2>
      <input className="rounded border px-3 py-2" placeholder="Admission ID" value={admissionId} onChange={(e) => setAdmissionId(e.target.value)} />
      <input className="rounded border px-3 py-2" placeholder="Nurse name" value={nurseName} onChange={(e) => setNurseName(e.target.value)} />
      <textarea className="rounded border px-3 py-2" placeholder="Note" value={note} onChange={(e) => setNote(e.target.value)} />
      <button className="rounded bg-slate-900 px-4 py-2 text-white" onClick={() => save.mutate()}>Save Note</button>
    </div>
  );
}
