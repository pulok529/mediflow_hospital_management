import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { aiApi } from "@/lib/api";

const featureLabelById: Record<number, string> = {
  1: "DoctorNoteDraft",
  2: "DischargeSummaryDraft",
  3: "LabResultExplanation",
  4: "DuplicatePatientDetection",
  5: "BillingAnomalyDetection",
  6: "InventoryDemandForecast"
};

export function AiAssistantPage() {
  const queryClient = useQueryClient();
  const drafts = useQuery({ queryKey: ["ai", "drafts"], queryFn: aiApi.listDrafts });

  const createDraft = useMutation({
    mutationFn: aiApi.createDraft,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["ai", "drafts"] })
  });

  const reviewDraft = useMutation({
    mutationFn: ({ id, approved, reviewerNotes }: { id: string; approved: boolean; reviewerNotes?: string }) =>
      aiApi.reviewDraft(id, approved, reviewerNotes),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ["ai", "drafts"] })
  });

  const onCreate = () => {
      createDraft.mutate({
      feature: 1,
      minimalContext: {
        patientId: "sample-patient-id",
        encounterId: "sample-encounter-id",
        symptoms: "fever, cough for 3 days"
      },
      promptVersion: "v1.0.0",
      modelVersion: "mediflow-safe-draft-v1"
    });
  };

  return (
    <div className="space-y-4 rounded-xl bg-white p-6 shadow-sm">
      <div>
        <h2 className="text-xl font-bold">AI Assistant Drafts</h2>
        <p className="text-sm text-slate-600">All outputs are draft-only and require human approval before use.</p>
      </div>

      <button className="rounded bg-slate-900 px-3 py-2 text-sm text-white" onClick={onCreate}>
        Create Sample Draft
      </button>

      <div className="grid gap-3">
        {(drafts.data ?? []).map((d: any) => (
          <div key={d.requestId} className="rounded border p-4">
            <p className="text-sm font-semibold">Feature: {featureLabelById[d.feature] ?? d.feature}</p>
            <p className="text-xs text-slate-500">Status: {d.approvalStatus === 1 ? "DraftPendingApproval" : d.approvalStatus === 2 ? "Approved" : "Rejected"}</p>
            <p className="mt-2 whitespace-pre-wrap text-sm">{d.draftOutput}</p>
            <p className="mt-2 text-xs text-amber-700">Safety: {(d.safetyLabels ?? []).join(", ")}</p>
            <p className="text-xs text-slate-500">Prompt {d.promptVersion} | Model {d.modelVersion}</p>
            {d.approvalStatus === 1 && (
              <div className="mt-3 flex gap-2">
                <button className="rounded bg-emerald-700 px-3 py-1 text-xs text-white" onClick={() => reviewDraft.mutate({ id: d.requestId, approved: true, reviewerNotes: "Clinically reviewed" })}>
                  Approve
                </button>
                <button className="rounded bg-rose-700 px-3 py-1 text-xs text-white" onClick={() => reviewDraft.mutate({ id: d.requestId, approved: false, reviewerNotes: "Needs rewrite" })}>
                  Reject
                </button>
              </div>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}
