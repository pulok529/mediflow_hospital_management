using Mediflow.Application.Abstractions.Audit;
using Mediflow.Application.Abstractions.Auth;
using Mediflow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mediflow.Infrastructure.Audit;

internal sealed class DatabaseAuditLogService(AppDbContext db) : IAuditLogService
{
    public void Add(Guid? actorUserId, string action, string entityType, string entityId, string details)
    {
        db.AuditLogs.Add(new AuditLogEntity
        {
            ActorUserId = actorUserId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details,
            AtUtc = DateTime.UtcNow
        });
        db.SaveChanges();
    }

    public IReadOnlyCollection<AuditLogDto> GetLatest(int take = 200)
        => db.AuditLogs
            .AsNoTracking()
            .OrderByDescending(x => x.AtUtc)
            .Take(take)
            .Select(x => new AuditLogDto(x.Id, x.ActorUserId, x.Action, x.EntityType, x.EntityId, x.AtUtc, x.Details))
            .ToArray();
}
