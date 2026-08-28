using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Creditos.Api.Configuration;
using Creditos.Api.Data;
using Creditos.Api.DTOs.Auth;
using Creditos.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Creditos.Api.Services;

public sealed class AuthService(
    AppDbContext dbContext,
    PasswordHasher<User> passwordHasher,
    IOptions<JwtOptions> jwtOptions,
    TimeProvider timeProvider) : IAuthService
{
    public async Task<LoginResponse?> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await dbContext.Users.AsNoTracking()
            .SingleOrDefaultAsync(item => item.Email == normalizedEmail, cancellationToken);

        if (user is null || !user.IsActive)
        {
            return null;
        }

        var verification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verification == PasswordVerificationResult.Failed)
        {
            return null;
        }

        var settings = jwtOptions.Value;
        var now = timeProvider.GetUtcNow();
        var expiresAt = now.AddMinutes(settings.ExpirationMinutes);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("display_name", user.DisplayName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return new LoginResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt,
            new AuthenticatedUser(user.Id, user.Email, user.DisplayName));
    }
}
