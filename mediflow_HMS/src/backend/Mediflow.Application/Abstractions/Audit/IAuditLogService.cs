using Mediflow.Application.Abstractions.Auth;

namespace Mediflow.Application.Abstractions.Audit;

public interface IAuditLogService
{
    void Add(Guid? actorUserId, string action, string entityType, string entityId, string details);
    IReadOnlyCollection<AuditLogDto> GetLatest(int take = 200);
}
