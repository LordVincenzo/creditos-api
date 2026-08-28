using Creditos.Api.DTOs.Auth;

namespace Creditos.Api.Services;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(string email, string password, CancellationToken cancellationToken);
}
