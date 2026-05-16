namespace Mediflow.Application.Abstractions.AI;

public enum AiFeature
{
    DoctorNoteDraft = 1,
    DischargeSummaryDraft = 2,
    LabResultExplanation = 3,
    DuplicatePatientDetection = 4,
    BillingAnomalyDetection = 5,
    InventoryDemandForecast = 6
}

public enum AiApprovalStatus
{
    DraftPendingApproval = 1,
    Approved = 2,
    Rejected = 3
}

public sealed record AiDraftRequest(
    AiFeature Feature,
    IReadOnlyDictionary<string, string> MinimalContext,
    string PromptVersion,
    string ModelVersion);

public sealed record AiDraftResult(
    Guid RequestId,
    AiFeature Feature,
    string DraftOutput,
    IReadOnlyCollection<string> SafetyLabels,
    string PromptVersion,
    string ModelVersion,
    AiApprovalStatus ApprovalStatus,
    DateTime RequestedAtUtc,
    DateTime? ReviewedAtUtc,
    Guid? ReviewedBy,
    string? ReviewerNotes);

public sealed record AiApprovalDecision(bool Approved, string? ReviewerNotes);

public sealed record AiAuditTrailEntry(DateTime AtUtc, Guid? ActorUserId, string Action, string Notes);

public sealed record AiRequestDetail(
    AiDraftResult Draft,
    IReadOnlyDictionary<string, string> MinimalContext,
    IReadOnlyCollection<AiAuditTrailEntry> AuditTrail);

public interface IAiWorkflowService
{
    AiDraftResult CreateDraft(AiDraftRequest request, Guid? actorUserId);
    IReadOnlyCollection<AiDraftResult> ListDrafts();
    AiRequestDetail? GetDraft(Guid requestId);
    AiDraftResult? ReviewDraft(Guid requestId, AiApprovalDecision decision, Guid? actorUserId);
}
