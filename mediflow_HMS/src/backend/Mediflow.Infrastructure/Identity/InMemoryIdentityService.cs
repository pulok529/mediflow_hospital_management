using Mediflow.Application.Abstractions.Audit;
using Mediflow.Application.Abstractions.Auth;
using Mediflow.Domain.Identity;

namespace Mediflow.Infrastructure.Identity;

internal sealed class InMemoryIdentityService(ITokenService tokenService, IAuditLogService auditLogService) : IIdentityService
{
    private static readonly object Sync = new();
    private static readonly List<string> PermissionCatalog =
    [
        "Patient.View", "Patient.Create", "Appointment.Create", "Billing.Refund", "Bed.Assign", "Discharge.Approve"
    ];

    private static readonly List<Role> Roles = [];
    private static readonly List<User> Users = [];
    private static readonly List<RefreshToken> RefreshTokens = [];

    static InMemoryIdentityService()
    {
        if (Roles.Count > 0) return;

        var seedRoleNames = new[]
        {
            "SuperAdmin", "Admin", "Receptionist", "Doctor", "Nurse", "LabTech", "Pharmacist",
            "BillingOfficer", "FloorReceptionist", "AdmissionOfficer"
        };

        foreach (var roleName in seedRoleNames)
        {
            var role = new Role(roleName);
            if (roleName is "SuperAdmin" or "Admin")
            {
                role.Permissions.AddRange(PermissionCatalog.Select(Permission.Of));
            }
            else if (roleName == "Receptionist")
            {
                role.Permissions.Add(Permission.Of("Patient.View"));
                role.Permissions.Add(Permission.Of("Patient.Create"));
                role.Permissions.Add(Permission.Of("Appointment.Create"));
            }
            Roles.Add(role);
        }

        var superAdminRole = Roles.First(r => r.Name == "SuperAdmin");
        var seedUser = new User("superadmin@mediflow.local", BCrypt.Net.BCrypt.HashPassword("SuperAdmin123!"));
        seedUser.Roles.Add(superAdminRole);
        Users.Add(seedUser);
    }

    public AuthTokens Login(string email, string password)
    {
        lock (Sync)
        {
            var user = Users.FirstOrDefault(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && x.IsActive);
            if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new InvalidOperationException("Invalid credentials.");

            var permissions = user.Roles.SelectMany(r => r.Permissions.Select(p => p.Code)).Distinct().ToArray();
            var roles = user.Roles.Select(r => r.Name).ToArray();
            var accessExpiry = DateTime.UtcNow.AddMinutes(30);
            var refreshExpiry = DateTime.UtcNow.AddDays(7);
            var refreshTokenValue = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            RefreshTokens.Add(new RefreshToken { UserId = user.Id, Token = refreshTokenValue, ExpiresAtUtc = refreshExpiry, Revoked = false });

            return new AuthTokens(tokenService.CreateToken(user.Id, user.Email, permissions, roles), refreshTokenValue, accessExpiry, refreshExpiry);
        }
    }

    public AuthTokens Refresh(string refreshToken)
    {
        lock (Sync)
        {
            var token = RefreshTokens.FirstOrDefault(x => x.Token == refreshToken && !x.Revoked && x.ExpiresAtUtc > DateTime.UtcNow)
                ?? throw new InvalidOperationException("Invalid refresh token.");
            token.Revoked = true;

            var user = Users.First(x => x.Id == token.UserId);
            var permissions = user.Roles.SelectMany(r => r.Permissions.Select(p => p.Code)).Distinct().ToArray();
            var roles = user.Roles.Select(r => r.Name).ToArray();
            var accessExpiry = DateTime.UtcNow.AddMinutes(30);
            var refreshExpiry = DateTime.UtcNow.AddDays(7);
            var newRefreshTokenValue = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            RefreshTokens.Add(new RefreshToken { UserId = user.Id, Token = newRefreshTokenValue, ExpiresAtUtc = refreshExpiry, Revoked = false });
            return new AuthTokens(tokenService.CreateToken(user.Id, user.Email, permissions, roles), newRefreshTokenValue, accessExpiry, refreshExpiry);
        }
    }

    public IReadOnlyCollection<UserDto> GetUsers() { lock (Sync) return Users.Select(MapUser).ToArray(); }
    public UserDto? GetUser(Guid userId) { lock (Sync) { var user = Users.FirstOrDefault(x => x.Id == userId); return user is null ? null : MapUser(user); } }

    public UserDto CreateUser(CreateUserRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            if (Users.Any(x => x.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Email already exists.");

            var user = new User(request.Email, BCrypt.Net.BCrypt.HashPassword(request.Password));
            ApplyRoles(user, request.Roles);
            Users.Add(user);
            auditLogService.Add(actorUserId, "User.Create", "User", user.Id.ToString(), $"Created user {user.Email}");
            return MapUser(user);
        }
    }

    public UserDto? UpdateUser(Guid userId, UpdateUserRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var user = Users.FirstOrDefault(x => x.Id == userId);
            if (user is null) return null;

            user.UpdateProfile(request.Email, request.IsActive);
            ApplyRoles(user, request.Roles);
            auditLogService.Add(actorUserId, "User.Update", "User", user.Id.ToString(), $"Updated user {user.Email}");
            return MapUser(user);
        }
    }

    public bool DeleteUser(Guid userId, Guid? actorUserId)
    {
        lock (Sync)
        {
            var user = Users.FirstOrDefault(x => x.Id == userId);
            if (user is null) return false;
            Users.Remove(user);
            auditLogService.Add(actorUserId, "User.Delete", "User", user.Id.ToString(), $"Deleted user {user.Email}");
            return true;
        }
    }

    public IReadOnlyCollection<RoleDto> GetRoles() { lock (Sync) return Roles.Select(MapRole).ToArray(); }

    public RoleDto CreateRole(CreateRoleRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            if (Roles.Any(x => x.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Role already exists.");

            var role = new Role(request.Name);
            Roles.Add(role);
            auditLogService.Add(actorUserId, "Role.Create", "Role", role.Id.ToString(), $"Created role {role.Name}");
            return MapRole(role);
        }
    }

    public RoleDto? UpdateRole(Guid roleId, UpdateRoleRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var role = Roles.FirstOrDefault(x => x.Id == roleId);
            if (role is null) return null;
            role.Rename(request.Name);
            auditLogService.Add(actorUserId, "Role.Update", "Role", role.Id.ToString(), $"Updated role {role.Name}");
            return MapRole(role);
        }
    }

    public bool DeleteRole(Guid roleId, Guid? actorUserId)
    {
        lock (Sync)
        {
            var role = Roles.FirstOrDefault(x => x.Id == roleId);
            if (role is null) return false;
            Roles.Remove(role);
            foreach (var user in Users) user.Roles.RemoveAll(r => r.Id == roleId);
            auditLogService.Add(actorUserId, "Role.Delete", "Role", roleId.ToString(), "Deleted role");
            return true;
        }
    }

    public IReadOnlyCollection<string> GetPermissions() => PermissionCatalog.ToArray();

    public RoleDto? AssignPermissions(Guid roleId, AssignPermissionsRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var role = Roles.FirstOrDefault(x => x.Id == roleId);
            if (role is null) return null;

            var validPermissions = request.Permissions.Where(PermissionCatalog.Contains).Distinct().ToArray();
            role.Permissions.Clear();
            role.Permissions.AddRange(validPermissions.Select(Permission.Of));
            auditLogService.Add(actorUserId, "Permission.Assign", "Role", role.Id.ToString(), $"Assigned permissions to {role.Name}");
            return MapRole(role);
        }
    }

    public IReadOnlyCollection<AuditLogDto> GetAuditLogs() => auditLogService.GetLatest();

    private static void ApplyRoles(User user, IReadOnlyCollection<string> roleNames)
    {
        user.Roles.Clear();
        foreach (var roleName in roleNames.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var role = Roles.FirstOrDefault(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            if (role is not null) user.Roles.Add(role);
        }
    }

    private static RoleDto MapRole(Role role) => new(role.Id, role.Name, role.Permissions.Select(p => p.Code).Distinct().ToArray());

    private static UserDto MapUser(User user)
    {
        var permissions = user.Roles.SelectMany(r => r.Permissions.Select(p => p.Code)).Distinct().ToArray();
        return new UserDto(user.Id, user.Email, user.IsActive, user.Roles.Select(r => r.Name).ToArray(), permissions);
    }
}
