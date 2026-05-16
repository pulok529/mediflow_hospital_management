namespace Mediflow.Application.Abstractions.Auth;

public interface IIdentityService
{
    AuthTokens Login(string email, string password);
    AuthTokens Refresh(string refreshToken);
    IReadOnlyCollection<UserDto> GetUsers();
    UserDto? GetUser(Guid userId);
    UserDto CreateUser(CreateUserRequest request, Guid? actorUserId);
    UserDto? UpdateUser(Guid userId, UpdateUserRequest request, Guid? actorUserId);
    bool DeleteUser(Guid userId, Guid? actorUserId);
    IReadOnlyCollection<RoleDto> GetRoles();
    RoleDto CreateRole(CreateRoleRequest request, Guid? actorUserId);
    RoleDto? UpdateRole(Guid roleId, UpdateRoleRequest request, Guid? actorUserId);
    bool DeleteRole(Guid roleId, Guid? actorUserId);
    IReadOnlyCollection<string> GetPermissions();
    RoleDto? AssignPermissions(Guid roleId, AssignPermissionsRequest request, Guid? actorUserId);
    IReadOnlyCollection<AuditLogDto> GetAuditLogs();
}

public sealed record UserDto(Guid Id, string Email, bool IsActive, IReadOnlyCollection<string> Roles, IReadOnlyCollection<string> Permissions);
public sealed record RoleDto(Guid Id, string Name, IReadOnlyCollection<string> Permissions);
public sealed record AuditLogDto(Guid Id, Guid? ActorUserId, string Action, string EntityType, string EntityId, DateTime AtUtc, string Details);

public sealed record CreateUserRequest(string Email, string Password, IReadOnlyCollection<string> Roles);
public sealed record UpdateUserRequest(string Email, bool IsActive, IReadOnlyCollection<string> Roles);
public sealed record CreateRoleRequest(string Name);
public sealed record UpdateRoleRequest(string Name);
public sealed record AssignPermissionsRequest(IReadOnlyCollection<string> Permissions);
