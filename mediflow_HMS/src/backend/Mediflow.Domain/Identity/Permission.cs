namespace Mediflow.Domain.Identity;

public sealed class Permission
{
    public string Code { get; }

    private Permission(string code) => Code = code;

    public static Permission Of(string code) => new(code);
}
