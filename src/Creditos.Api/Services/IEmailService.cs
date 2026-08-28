namespace Creditos.Api.Services;

public sealed record EmailMessage(string To, string Subject, string HtmlBody, string TextBody);

public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}
