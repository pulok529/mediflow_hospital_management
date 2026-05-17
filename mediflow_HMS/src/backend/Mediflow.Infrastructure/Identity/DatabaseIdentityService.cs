using Mediflow.Application.Abstractions.Audit;
using Mediflow.Application.Abstractions.Auth;
using Mediflow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mediflow.Infrastructure.Identity;

internal sealed class DatabaseIdentityService(AppDbContext db, ITokenService tokenService, IAuditLogService auditLogService) : IIdentityService
{
    private static readonly string[] PermissionCatalog =
    [
        "Patient.View", "Patient.Create", "Appointment.Create", "Billing.Refund", "Bed.Assign",
        "Discharge.Approve", "Consultation.Manage", "Lab.Result.Approve", "Radiology.Report.Approve",
        "Pharmacy.Stock.Manage", "Report.Export", "AI.Review"
    ];

    private static readonly string[] SeedRoleNames =
    [
        "SuperAdmin", "Admin", "Receptionist", "Doctor", "Nurse", "LabTech", "Radiologist",
        "Pharmacist", "BillingOfficer", "FloorReceptionist", "AdmissionOfficer"
    ];

    public AuthTokens Login(string email, string password)
    {
        EnsureSeeded();
        var user = db.Users
            .Include(x => x.Roles)
            .ThenInclude(x => x.Permissions)
            .FirstOrDefault(x => x.Email == email && x.IsActive);

        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            throw new InvalidOperationException("Invalid credentials.");

        var permissions = user.Roles.SelectMany(r => r.Permissions.Select(p => p.Code)).Distinct().ToArray();
        var roles = user.Roles.Select(r => r.Name).ToArray();
        var accessExpiry = DateTime.UtcNow.AddMinutes(30);
        var refreshExpiry = DateTime.UtcNow.AddDays(7);
        var refreshTokenValue = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        db.RefreshTokens.Add(new RefreshTokenEntity
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAtUtc = refreshExpiry,
            Revoked = false
        });
        db.SaveChanges();

        return new AuthTokens(tokenService.CreateToken(user.Id, user.Email, permissions, roles), refreshTokenValue, accessExpiry, refreshExpiry);
    }

    public AuthTokens Refresh(string refreshToken)
    {
        EnsureSeeded();
        var token = db.RefreshTokens.FirstOrDefault(x => x.Token == refreshToken && !x.Revoked && x.ExpiresAtUtc > DateTime.UtcNow)
            ?? throw new InvalidOperationException("Invalid refresh token.");

        token.Revoked = true;
        var user = db.Users
            .Include(x => x.Roles)
            .ThenInclude(x => x.Permissions)
            .First(x => x.Id == token.UserId);

        var permissions = user.Roles.SelectMany(r => r.Permissions.Select(p => p.Code)).Distinct().ToArray();
        var roles = user.Roles.Select(r => r.Name).ToArray();
        var accessExpiry = DateTime.UtcNow.AddMinutes(30);
        var refreshExpiry = DateTime.UtcNow.AddDays(7);
        var newRefreshTokenValue = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        db.RefreshTokens.Add(new RefreshTokenEntity
        {
            UserId = user.Id,
            Token = newRefreshTokenValue,
            ExpiresAtUtc = refreshExpiry,
            Revoked = false
        });
        db.SaveChanges();

        return new AuthTokens(tokenService.CreateToken(user.Id, user.Email, permissions, roles), newRefreshTokenValue, accessExpiry, refreshExpiry);
    }

    public IReadOnlyCollection<UserDto> GetUsers()
    {
        EnsureSeeded();
        return db.Users.Include(x => x.Roles).ThenInclude(x => x.Permissions).AsNoTracking().Select(MapUser).ToArray();
    }

    public UserDto? GetUser(Guid userId)
    {
        EnsureSeeded();
        var user = db.Users.Include(x => x.Roles).ThenInclude(x => x.Permissions).AsNoTracking().FirstOrDefault(x => x.Id == userId);
        return user is null ? null : MapUser(user);
    }

    public UserDto CreateUser(CreateUserRequest request, Guid? actorUserId)
    {
        EnsureSeeded();
        if (db.Users.Any(x => x.Email == request.Email))
            throw new InvalidOperationException("Email already exists.");

        var user = new UserEntity
        {
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        ApplyRoles(user, request.Roles);
        db.Users.Add(user);
        db.SaveChanges();
        auditLogService.Add(actorUserId, "User.Create", "User", user.Id.ToString(), $"Created user {user.Email}");
        return MapUser(user);
    }

    public UserDto? UpdateUser(Guid userId, UpdateUserRequest request, Guid? actorUserId)
    {
        EnsureSeeded();
        var user = db.Users.Include(x => x.Roles).ThenInclude(x => x.Permissions).FirstOrDefault(x => x.Id == userId);
        if (user is null) return null;

        user.Email = request.Email;
        user.IsActive = request.IsActive;
        ApplyRoles(user, request.Roles);
        db.SaveChanges();
        auditLogService.Add(actorUserId, "User.Update", "User", user.Id.ToString(), $"Updated user {user.Email}");
        return MapUser(user);
    }

    public bool DeleteUser(Guid userId, Guid? actorUserId)
    {
        EnsureSeeded();
        var user = db.Users.FirstOrDefault(x => x.Id == userId);
        if (user is null) return false;

        db.Users.Remove(user);
        db.SaveChanges();
        auditLogService.Add(actorUserId, "User.Delete", "User", userId.ToString(), "Deleted user");
        return true;
    }

    public IReadOnlyCollection<RoleDto> GetRoles()
    {
        EnsureSeeded();
        return db.Roles.Include(x => x.Permissions).AsNoTracking().Select(MapRole).ToArray();
    }

    public RoleDto CreateRole(CreateRoleRequest request, Guid? actorUserId)
    {
        EnsureSeeded();
        if (db.Roles.Any(x => x.Name == request.Name))
            throw new InvalidOperationException("Role already exists.");

        var role = new RoleEntity { Name = request.Name };
        db.Roles.Add(role);
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Role.Create", "Role", role.Id.ToString(), $"Created role {role.Name}");
        return MapRole(role);
    }

    public RoleDto? UpdateRole(Guid roleId, UpdateRoleRequest request, Guid? actorUserId)
    {
        EnsureSeeded();
        var role = db.Roles.Include(x => x.Permissions).FirstOrDefault(x => x.Id == roleId);
        if (role is null) return null;

        role.Name = request.Name;
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Role.Update", "Role", role.Id.ToString(), $"Updated role {role.Name}");
        return MapRole(role);
    }

    public bool DeleteRole(Guid roleId, Guid? actorUserId)
    {
        EnsureSeeded();
        var role = db.Roles.FirstOrDefault(x => x.Id == roleId);
        if (role is null) return false;

        db.Roles.Remove(role);
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Role.Delete", "Role", roleId.ToString(), "Deleted role");
        return true;
    }

    public IReadOnlyCollection<string> GetPermissions() => PermissionCatalog;

    public RoleDto? AssignPermissions(Guid roleId, AssignPermissionsRequest request, Guid? actorUserId)
    {
        EnsureSeeded();
        var role = db.Roles.Include(x => x.Permissions).FirstOrDefault(x => x.Id == roleId);
        if (role is null) return null;

        var validPermissions = request.Permissions.Where(PermissionCatalog.Contains).Distinct().ToArray();
        var permissionEntities = db.Permissions.Where(x => validPermissions.Contains(x.Code)).ToArray();
        role.Permissions.Clear();
        role.Permissions.AddRange(permissionEntities);
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Permission.Assign", "Role", role.Id.ToString(), $"Assigned permissions to {role.Name}");
        return MapRole(role);
    }

    public IReadOnlyCollection<AuditLogDto> GetAuditLogs() => auditLogService.GetLatest();

    private void EnsureSeeded()
    {
        if (!db.Permissions.Any())
        {
            db.Permissions.AddRange(PermissionCatalog.Select(x => new PermissionEntity { Code = x }));
            db.SaveChanges();
        }

        foreach (var roleName in SeedRoleNames)
        {
            if (!db.Roles.Any(x => x.Name == roleName))
            {
                var role = new RoleEntity { Name = roleName };
                if (roleName is "SuperAdmin" or "Admin")
                {
                    role.Permissions.AddRange(db.Permissions);
                }
                else if (roleName == "Doctor")
                {
                    AddPermission(role, "Patient.View");
                    AddPermission(role, "Consultation.Manage");
                    AddPermission(role, "AI.Review");
                }
                else if (roleName == "Receptionist")
                {
                    AddPermission(role, "Patient.View");
                    AddPermission(role, "Patient.Create");
                    AddPermission(role, "Appointment.Create");
                }

                db.Roles.Add(role);
                db.SaveChanges();
            }
        }

        if (!db.Users.Any(x => x.Email == "superadmin@mediflow.local"))
        {
            var superAdminRole = db.Roles.Include(x => x.Permissions).First(r => r.Name == "SuperAdmin");
            db.Users.Add(new UserEntity
            {
                Email = "superadmin@mediflow.local",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("SuperAdmin123!"),
                Roles = [superAdminRole]
            });
            db.SaveChanges();
        }
    }

    private void ApplyRoles(UserEntity user, IReadOnlyCollection<string> roleNames)
    {
        var roles = db.Roles.Include(x => x.Permissions).Where(x => roleNames.Contains(x.Name)).ToArray();
        user.Roles.Clear();
        user.Roles.AddRange(roles);
    }

    private void AddPermission(RoleEntity role, string code)
    {
        var permission = db.Permissions.First(x => x.Code == code);
        role.Permissions.Add(permission);
    }

    private static RoleDto MapRole(RoleEntity role)
        => new(role.Id, role.Name, role.Permissions.Select(p => p.Code).Distinct().ToArray());

    private static UserDto MapUser(UserEntity user)
    {
        var permissions = user.Roles.SelectMany(r => r.Permissions.Select(p => p.Code)).Distinct().ToArray();
        return new UserDto(user.Id, user.Email, user.IsActive, user.Roles.Select(r => r.Name).ToArray(), permissions);
    }
}
