using Creditos.Api.Configuration;
using Creditos.Api.Entities;
using Creditos.Api.Jobs;
using Creditos.Api.Services;
using Microsoft.Extensions.Options;

namespace Creditos.Api.Tests.Jobs;

public class CreditEmailJobTests
{
    private sealed class RecordingEmailService : IEmailService
    {
        public EmailMessage? Message { get; private set; }
        public Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            Message = message;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task JobBuildsRequiredCreditNotificationContent()
    {
        await using var db = TestDb.Create();
        var user = new User { Email = "juan@demo.local", DisplayName = "Juan Perez", PasswordHash = "hash", IsActive = true };
        var credit = new Credit { ClientName = "Pepito Perez", ClientDocument = "123", Amount = 7_800_000m, InterestRate = 2m, TermMonths = 10, RegisteredByUserId = user.Id, CommercialNameSnapshot = "Juan Perez", CreatedAtUtc = new DateTimeOffset(2026, 8, 27, 20, 0, 0, TimeSpan.Zero) };
        db.Users.Add(user);
        db.Credits.Add(credit);
        await db.SaveChangesAsync();
        var email = new RecordingEmailService();
        var job = new SendCreditRegisteredEmailJob(db, email, Options.Create(new CreditNotificationOptions { RecipientEmail = "creditos@gmail.com" }));

        await job.ExecuteAsync(credit.Id, CancellationToken.None);

        Assert.NotNull(email.Message);
        Assert.Equal("creditos@gmail.com", email.Message!.To);
        Assert.Contains("Pepito Perez", email.Message.HtmlBody);
        Assert.Contains("Juan Perez", email.Message.HtmlBody);
        Assert.Contains("7800000", email.Message.TextBody.Replace(".", string.Empty).Replace(",", string.Empty));
    }
}
