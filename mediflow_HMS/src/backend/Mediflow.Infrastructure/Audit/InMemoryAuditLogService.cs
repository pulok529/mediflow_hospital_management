using Mediflow.Application.Abstractions.Audit;
using Mediflow.Application.Abstractions.Auth;
using Mediflow.Domain.Identity;

namespace Mediflow.Infrastructure.Audit;

internal sealed class InMemoryAuditLogService : IAuditLogService
{
    private static readonly object Sync = new();
    private static readonly List<AuditLog> Logs = [];

    public void Add(Guid? actorUserId, string action, string entityType, string entityId, string details)
    {
        lock (Sync)
        {
            Logs.Add(new AuditLog
            {
                ActorUserId = actorUserId,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Details = details,
                AtUtc = DateTime.UtcNow
            });
        }
    }

    public IReadOnlyCollection<AuditLogDto> GetLatest(int take = 200)
    {
        lock (Sync)
        {
            return Logs.OrderByDescending(x => x.AtUtc).Take(take)
                .Select(x => new AuditLogDto(x.Id, x.ActorUserId, x.Action, x.EntityType, x.EntityId, x.AtUtc, x.Details))
                .ToArray();
        }
    }
}
