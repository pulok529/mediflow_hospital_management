using Mediflow.Domain.Common;

namespace Mediflow.Domain.Identity;

public sealed class Role : BaseEntity
{
    public string Name { get; private set; }
    public List<Permission> Permissions { get; private set; } = new();

    public Role(string name)
    {
        Name = name;
    }
}
