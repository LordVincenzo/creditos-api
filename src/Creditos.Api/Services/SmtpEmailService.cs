using Creditos.Api.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Creditos.Api.Services;

public sealed class SmtpEmailService(IOptions<SmtpOptions> smtpOptions) : IEmailService
{
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var options = smtpOptions.Value;
        var mime = new MimeMessage();
        mime.From.Add(new MailboxAddress(options.FromName, options.FromEmail));
        mime.To.Add(MailboxAddress.Parse(message.To));
        mime.Subject = message.Subject;
        mime.Body = new BodyBuilder { HtmlBody = message.HtmlBody, TextBody = message.TextBody }.ToMessageBody();

        var socketOptions = options.UseSsl
            ? SecureSocketOptions.SslOnConnect
            : options.UseStartTls
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.None;

        using var client = new SmtpClient();
        await client.ConnectAsync(options.Host, options.Port, socketOptions, cancellationToken);
        if (!string.IsNullOrWhiteSpace(options.Username))
        {
            await client.AuthenticateAsync(options.Username, options.Password, cancellationToken);
        }
        await client.SendAsync(mime, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
