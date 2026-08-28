using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Creditos.Api.Data;
using Creditos.Api.DTOs.Auth;
using Creditos.Api.DTOs.Common;
using Creditos.Api.DTOs.Credits;
using Creditos.Api.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Creditos.Api.Tests.Api;

public class ApiFlowTests
{
    private sealed class ApiFactory : WebApplicationFactory<Program>
    {
        private readonly string _databaseName = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<AppDbContext>>();
                services.RemoveAll<AppDbContext>();
                services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(_databaseName));
            });
        }

        public async Task SeedCommercialAsync()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            if (await db.Users.AnyAsync()) return;

            var user = new User
            {
                Email = "comercial1@demo.local",
                DisplayName = "Comercial Uno",
                IsActive = true
            };
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "Demo1234!");
            db.Users.Add(user);
            await db.SaveChangesAsync();
        }
    }

    private static async Task<string> LoginAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email = "comercial1@demo.local", password = "Demo1234!" });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        return body!.AccessToken;
    }

    [Fact]
    public async Task LoginValidAndInvalidReturnExpectedStatuses()
    {
        using var factory = new ApiFactory();
        await factory.SeedCommercialAsync();
        var client = factory.CreateClient();

        var valid = await client.PostAsJsonAsync("/api/auth/login", new { email = "comercial1@demo.local", password = "Demo1234!" });
        var invalid = await client.PostAsJsonAsync("/api/auth/login", new { email = "comercial1@demo.local", password = "wrong-password" });

        Assert.Equal(HttpStatusCode.OK, valid.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, invalid.StatusCode);
    }

    [Fact]
    public async Task CreateValidReturns201AndInvalidReturns400()
    {
        using var factory = new ApiFactory();
        await factory.SeedCommercialAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await LoginAsync(client));

        var valid = await client.PostAsJsonAsync("/api/credits", new
        {
            clientName = "Pepito Perez",
            clientDocument = "123456789",
            amount = 7800000m,
            interestRate = 2m,
            termMonths = 10
        });
        var invalid = await client.PostAsJsonAsync("/api/credits", new
        {
            clientName = "Pepito Perez",
            clientDocument = "123456789",
            amount = -1m,
            interestRate = 2m,
            termMonths = 10
        });

        Assert.Equal(HttpStatusCode.Created, valid.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
    }

    [Fact]
    public async Task QueryReturnsCreatedCredit()
    {
        using var factory = new ApiFactory();
        await factory.SeedCommercialAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await LoginAsync(client));
        await client.PostAsJsonAsync("/api/credits", new
        {
            clientName = "Maria Lopez",
            clientDocument = "998877",
            amount = 500000m,
            interestRate = 1.5m,
            termMonths = 12
        });

        var result = await client.GetFromJsonAsync<PagedResult<CreditResponse>>("/api/credits?clientName=maria&page=1&pageSize=20");

        Assert.NotNull(result);
        Assert.Contains(result!.Items, item => item.ClientName == "Maria Lopez");
    }
}

