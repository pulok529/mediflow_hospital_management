namespace Mediflow.Api.Contracts.Identity;

public sealed record CreateUserRequest(string Email, string Password, IReadOnlyCollection<string> Roles);
public sealed record UpdateUserRequest(string Email, bool IsActive, IReadOnlyCollection<string> Roles);
public sealed record CreateRoleRequest(string Name);
public sealed record UpdateRoleRequest(string Name);
public sealed record AssignPermissionsRequest(IReadOnlyCollection<string> Permissions);
