using Creditos.Api.Configuration;
using Creditos.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Creditos.Api.Data;

public sealed class DevelopmentDataSeeder(
    AppDbContext dbContext,
    IHostEnvironment environment,
    IOptions<DemoUsersOptions> demoOptions,
    PasswordHasher<User> passwordHasher,
    ILogger<DevelopmentDataSeeder> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!environment.IsDevelopment() || !demoOptions.Value.Enabled)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(demoOptions.Value.Password))
        {
            throw new InvalidOperationException("DemoUsers:Password must be configured when demo seeding is enabled.");
        }

        var users = new[]
        {
            (Email: "comercial1@demo.local", DisplayName: "Comercial Uno"),
            (Email: "comercial2@demo.local", DisplayName: "Comercial Dos")
        };

        foreach (var item in users)
        {
            var email = item.Email.ToLowerInvariant();
            if (await dbContext.Users.AnyAsync(user => user.Email == email, cancellationToken))
            {
                continue;
            }

            var user = new User
            {
                Email = email,
                DisplayName = item.DisplayName,
                IsActive = true,
                CreatedAtUtc = DateTimeOffset.UtcNow
            };
            user.PasswordHash = passwordHasher.HashPassword(user, demoOptions.Value.Password);
            dbContext.Users.Add(user);
        }

        if (dbContext.ChangeTracker.HasChanges())
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Development demo users were created.");
        }
    }
}
