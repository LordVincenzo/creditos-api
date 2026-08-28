namespace Creditos.Api.Jobs;

public interface ICreditNotificationQueue
{
    void Enqueue(Guid creditId);
}
