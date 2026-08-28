namespace Creditos.Api.Jobs;

public sealed class NoOpCreditNotificationQueue : ICreditNotificationQueue
{
    public void Enqueue(Guid creditId) { }
}
