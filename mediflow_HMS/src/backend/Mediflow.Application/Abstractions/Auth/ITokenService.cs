namespace Mediflow.Application.Abstractions.Auth;

public interface ITokenService
{
    string CreateToken(Guid userId, string email, IReadOnlyCollection<string> permissions, IReadOnlyCollection<string> roles);
}
