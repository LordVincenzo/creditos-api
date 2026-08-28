using Hangfire;

namespace Creditos.Api.Jobs;

public sealed class HangfireCreditNotificationQueue(IBackgroundJobClient jobs) : ICreditNotificationQueue
{
    public void Enqueue(Guid creditId)
    {
        jobs.Enqueue<SendCreditRegisteredEmailJob>(job => job.ExecuteAsync(creditId, CancellationToken.None));
    }
}
