using Creditos.Api.DTOs.Credits;
using Creditos.Api.Entities;
using Creditos.Api.Jobs;
using Creditos.Api.Services;

namespace Creditos.Api.Tests.Credits;

public class CreditQueryTests
{
    private sealed class NoopQueue : ICreditNotificationQueue { public void Enqueue(Guid creditId) { } }

    private static async Task<CreditService> CreateSeededService()
    {
        var db = TestDb.Create();
        var user = new User { Email = "juan@demo.local", DisplayName = "Juan Perez", PasswordHash = "hash", IsActive = true };
        db.Users.Add(user);
        db.Credits.AddRange(
            new Credit { ClientName = "Pepito Perez", ClientDocument = "12345", Amount = 900m, InterestRate = 2m, TermMonths = 10, RegisteredByUserId = user.Id, CommercialNameSnapshot = "Juan Perez", CreatedAtUtc = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero) },
            new Credit { ClientName = "Maria Lopez", ClientDocument = "99887", Amount = 100m, InterestRate = 1m, TermMonths = 6, RegisteredByUserId = user.Id, CommercialNameSnapshot = "Juan Perez", CreatedAtUtc = new DateTimeOffset(2026, 2, 1, 0, 0, 0, TimeSpan.Zero) }
        );
        await db.SaveChangesAsync();
        return new CreditService(db, new NoopQueue(), TimeProvider.System);
    }

    [Theory]
    [InlineData("pepito", null, null, "Pepito Perez")]
    [InlineData(null, "998", null, "Maria Lopez")]
    [InlineData(null, null, "juan", "Pepito Perez")]
    public async Task TextFiltersReturnExpectedCredit(string? name, string? document, string? commercial, string expectedClient)
    {
        var service = await CreateSeededService();
        var result = await service.QueryAsync(new CreditQuery { ClientName = name, ClientDocument = document, Commercial = commercial }, CancellationToken.None);
        Assert.Contains(result.Items, item => item.ClientName == expectedClient);
    }

    [Fact]
    public async Task SortByAmountAscendingWorks()
    {
        var service = await CreateSeededService();
        var result = await service.QueryAsync(new CreditQuery { SortBy = "amount", SortDirection = "asc" }, CancellationToken.None);
        Assert.Equal(new[] { 100m, 900m }, result.Items.Select(item => item.Amount).ToArray());
    }

    [Fact]
    public async Task SortByCreatedAtDescendingWorks()
    {
        var service = await CreateSeededService();
        var result = await service.QueryAsync(new CreditQuery { SortBy = "createdAt", SortDirection = "desc" }, CancellationToken.None);
        Assert.Equal("Maria Lopez", result.Items.First().ClientName);
    }

    [Fact]
    public async Task PageSizeIsCappedAtOneHundred()
    {
        var service = await CreateSeededService();
        var result = await service.QueryAsync(new CreditQuery { PageSize = 999 }, CancellationToken.None);
        Assert.Equal(100, result.PageSize);
    }
}
