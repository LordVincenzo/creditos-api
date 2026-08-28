using Creditos.Api.DTOs.Credits;
using Creditos.Api.Entities;
using Creditos.Api.Jobs;
using Creditos.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Creditos.Api.Tests.Credits;

public class CreditCreationTests
{
    private sealed class RecordingQueue : ICreditNotificationQueue
    {
        public Guid? CreditId { get; private set; }
        public void Enqueue(Guid creditId) => CreditId = creditId;
    }

    [Fact]
    public async Task CreateUsesAuthenticatedCommercialAndPersistsCredit()
    {
        await using var db = TestDb.Create();
        var user = new User { Email = "sales@demo.local", DisplayName = "Juan Perez", PasswordHash = "hash", IsActive = true };
        db.Users.Add(user);
        await db.SaveChangesAsync();
        var queue = new RecordingQueue();
        var service = new CreditService(db, queue, TimeProvider.System);

        var result = await service.CreateAsync(new CreateCreditRequest
        {
            ClientName = "  Pepito Perez  ",
            ClientDocument = " 0012345 ",
            Amount = 7_800_000m,
            InterestRate = 2m,
            TermMonths = 10
        }, user.Id, CancellationToken.None);

        var stored = await db.Credits.SingleAsync();
        Assert.Equal(user.Id, stored.RegisteredByUserId);
        Assert.Equal("Juan Perez", stored.CommercialNameSnapshot);
        Assert.Equal("Pepito Perez", stored.ClientName);
        Assert.Equal("0012345", stored.ClientDocument);
        Assert.Equal(stored.Id, queue.CreditId);
        Assert.Equal(stored.Id, result.Id);
    }

    [Fact]
    public async Task MissingAuthenticatedUserIsRejected()
    {
        await using var db = TestDb.Create();
        var service = new CreditService(db, new RecordingQueue(), TimeProvider.System);
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.CreateAsync(new CreateCreditRequest
        {
            ClientName = "Cliente",
            ClientDocument = "123",
            Amount = 100m,
            InterestRate = 1m,
            TermMonths = 3
        }, Guid.NewGuid(), CancellationToken.None));
    }
}
