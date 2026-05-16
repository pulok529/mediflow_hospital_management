namespace Mediflow.Application.Abstractions.Auth;

public sealed record AuthTokens(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAtUtc, DateTime RefreshTokenExpiresAtUtc);
