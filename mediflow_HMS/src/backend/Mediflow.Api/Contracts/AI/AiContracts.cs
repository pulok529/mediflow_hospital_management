using Mediflow.Application.Abstractions.AI;

namespace Mediflow.Api.Contracts.AI;

public sealed record AiDraftRequest(
    AiFeature Feature,
    IReadOnlyDictionary<string, string> MinimalContext,
    string PromptVersion,
    string ModelVersion);

public sealed record AiApprovalDecision(
    bool Approved,
    string? ReviewerNotes);
