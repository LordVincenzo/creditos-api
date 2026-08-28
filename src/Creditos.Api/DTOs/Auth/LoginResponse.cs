namespace Creditos.Api.DTOs.Auth;

public sealed record AuthenticatedUser(Guid Id, string Email, string DisplayName);

public sealed record LoginResponse(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    AuthenticatedUser User);
