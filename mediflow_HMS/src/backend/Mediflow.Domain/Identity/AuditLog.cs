namespace Mediflow.Domain.Identity;

public sealed class AuditLog
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid? ActorUserId { get; init; }
    public string Action { get; init; } = string.Empty;
    public string EntityType { get; init; } = string.Empty;
    public string EntityId { get; init; } = string.Empty;
    public DateTime AtUtc { get; init; } = DateTime.UtcNow;
    public string Details { get; init; } = string.Empty;
}
