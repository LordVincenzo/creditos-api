using Creditos.Api.Configuration;
using Creditos.Api.Entities;
using Creditos.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Creditos.Api.Tests.Auth;

public class AuthServiceTests
{
    private static AuthService CreateService(Creditos.Api.Data.AppDbContext db)
    {
        var jwt = Options.Create(new JwtOptions
        {
            Issuer = "creditos-api-tests",
            Audience = "creditos-mobile-tests",
            Key = "tests-only-jwt-signing-key-32-chars-minimum",
            ExpirationMinutes = 60
        });
        return new AuthService(db, new PasswordHasher<User>(), jwt, TimeProvider.System);
    }

    [Fact]
    public async Task ValidLoginReturnsJwtAndPublicUser()
    {
        await using var db = TestDb.Create();
        var user = new User { Email = "comercial1@demo.local", DisplayName = "Comercial Uno", IsActive = true };
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "Demo1234!");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var result = await CreateService(db).LoginAsync(user.Email, "Demo1234!", CancellationToken.None);

        Assert.NotNull(result);
        Assert.False(string.IsNullOrWhiteSpace(result!.AccessToken));
        Assert.Equal(user.Id, result.User.Id);
        Assert.Equal("Comercial Uno", result.User.DisplayName);
    }

    [Fact]
    public async Task InvalidPasswordReturnsNull()
    {
        await using var db = TestDb.Create();
        var user = new User { Email = "comercial1@demo.local", DisplayName = "Comercial Uno", IsActive = true };
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "Demo1234!");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var result = await CreateService(db).LoginAsync(user.Email, "wrong", CancellationToken.None);
        Assert.Null(result);
    }

    [Fact]
    public async Task InactiveUserCannotLogin()
    {
        await using var db = TestDb.Create();
        var user = new User { Email = "inactive@demo.local", DisplayName = "Inactive", IsActive = false };
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "Demo1234!");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var result = await CreateService(db).LoginAsync(user.Email, "Demo1234!", CancellationToken.None);
        Assert.Null(result);
    }
}
