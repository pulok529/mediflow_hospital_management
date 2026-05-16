using Mediflow.Domain.Common;

namespace Mediflow.Domain.Identity;

public sealed class User : BaseEntity
{
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public bool IsActive { get; private set; } = true;
    public List<Role> Roles { get; private set; } = new();

    public User(string email, string passwordHash)
    {
        Email = email;
        PasswordHash = passwordHash;
    }

    public void UpdateProfile(string email, bool isActive)
    {
        Email = email;
        IsActive = isActive;
    }
}
