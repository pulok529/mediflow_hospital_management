using Mediflow.Application.Abstractions.AI;
using Mediflow.Application.Abstractions.Audit;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Json;

namespace Mediflow.Infrastructure.AI;

internal sealed class FastApiAiWorkflowService(IHttpClientFactory httpClientFactory, IAuditLogService auditLogService) : IAiWorkflowService
{
    private static readonly ConcurrentDictionary<Guid, StoredAiRequest> Requests = new();

    public AiDraftResult CreateDraft(AiDraftRequest request, Guid? actorUserId)
    {
        var aiResponse = GenerateDraftFromAiService(request);

        var now = DateTime.UtcNow;
        var stored = new StoredAiRequest(
            Id: Guid.NewGuid(),
            Feature: request.Feature,
            MinimalContext: request.MinimalContext,
            DraftOutput: aiResponse.DraftOutput,
            SafetyLabels: aiResponse.SafetyLabels,
            PromptVersion: request.PromptVersion,
            ModelVersion: request.ModelVersion,
            ApprovalStatus: AiApprovalStatus.DraftPendingApproval,
            RequestedAtUtc: now,
            ReviewedAtUtc: null,
            ReviewedBy: null,
            ReviewerNotes: null,
            AuditTrail: [new AiAuditTrailEntry(now, actorUserId, "AI_DRAFT_REQUESTED", "Draft generated and pending human approval")]
        );

        Requests[stored.Id] = stored;
        auditLogService.Add(actorUserId, "AI_DRAFT_REQUESTED", "AiDraft", stored.Id.ToString(), $"Feature={request.Feature}; PromptVersion={request.PromptVersion}; ModelVersion={request.ModelVersion}");
        return ToDraftResult(stored);
    }

    public IReadOnlyCollection<AiDraftResult> ListDrafts() => Requests.Values
        .OrderByDescending(x => x.RequestedAtUtc)
        .Select(ToDraftResult)
        .ToArray();

    public AiRequestDetail? GetDraft(Guid requestId)
    {
        if (!Requests.TryGetValue(requestId, out var stored))
        {
            return null;
        }

        return new AiRequestDetail(ToDraftResult(stored), stored.MinimalContext, stored.AuditTrail);
    }

    public AiDraftResult? ReviewDraft(Guid requestId, AiApprovalDecision decision, Guid? actorUserId)
    {
        if (!Requests.TryGetValue(requestId, out var stored))
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var status = decision.Approved ? AiApprovalStatus.Approved : AiApprovalStatus.Rejected;
        var action = decision.Approved ? "AI_DRAFT_APPROVED" : "AI_DRAFT_REJECTED";

        var updated = stored with
        {
            ApprovalStatus = status,
            ReviewedAtUtc = now,
            ReviewedBy = actorUserId,
            ReviewerNotes = decision.ReviewerNotes,
            AuditTrail = stored.AuditTrail
                .Append(new AiAuditTrailEntry(now, actorUserId, action, decision.ReviewerNotes ?? string.Empty))
                .ToArray()
        };

        Requests[requestId] = updated;
        auditLogService.Add(actorUserId, action, "AiDraft", requestId.ToString(), $"Status={status}; Notes={decision.ReviewerNotes}");
        return ToDraftResult(updated);
    }

    private AiServiceResponse GenerateDraftFromAiService(AiDraftRequest request)
    {
        var client = httpClientFactory.CreateClient("ai-service");
        var response = client.PostAsJsonAsync("/v1/generate", new
        {
            feature = request.Feature.ToString(),
            minimalContext = request.MinimalContext,
            promptVersion = request.PromptVersion,
            modelVersion = request.ModelVersion
        }).GetAwaiter().GetResult();

        if (!response.IsSuccessStatusCode)
        {
            return BuildFallback(request.Feature);
        }

        var payload = response.Content.ReadFromJsonAsync<AiServiceResponse>().GetAwaiter().GetResult();
        return payload ?? BuildFallback(request.Feature);
    }

    private static AiServiceResponse BuildFallback(AiFeature feature)
        => new($"[DRAFT ONLY] AI service unavailable. Manual draft for feature {feature} is required.", ["DRAFT_ONLY", "HUMAN_REVIEW_REQUIRED", "SERVICE_FALLBACK"]);

    private static AiDraftResult ToDraftResult(StoredAiRequest request)
        => new(
            request.Id,
            request.Feature,
            request.DraftOutput,
            request.SafetyLabels,
            request.PromptVersion,
            request.ModelVersion,
            request.ApprovalStatus,
            request.RequestedAtUtc,
            request.ReviewedAtUtc,
            request.ReviewedBy,
            request.ReviewerNotes);

    private sealed record StoredAiRequest(
        Guid Id,
        AiFeature Feature,
        IReadOnlyDictionary<string, string> MinimalContext,
        string DraftOutput,
        IReadOnlyCollection<string> SafetyLabels,
        string PromptVersion,
        string ModelVersion,
        AiApprovalStatus ApprovalStatus,
        DateTime RequestedAtUtc,
        DateTime? ReviewedAtUtc,
        Guid? ReviewedBy,
        string? ReviewerNotes,
        IReadOnlyCollection<AiAuditTrailEntry> AuditTrail);

    private sealed record AiServiceResponse(string DraftOutput, IReadOnlyCollection<string> SafetyLabels);
}
