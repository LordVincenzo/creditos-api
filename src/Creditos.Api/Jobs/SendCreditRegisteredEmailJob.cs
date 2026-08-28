using System.Globalization;
using System.Net;
using Creditos.Api.Configuration;
using Creditos.Api.Data;
using Creditos.Api.Services;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Creditos.Api.Jobs;

public sealed class SendCreditRegisteredEmailJob(
    AppDbContext dbContext,
    IEmailService emailService,
    IOptions<CreditNotificationOptions> notificationOptions)
{
    [AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
    public async Task ExecuteAsync(Guid creditId, CancellationToken cancellationToken)
    {
        var credit = await dbContext.Credits.AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == creditId, cancellationToken);
        if (credit is null)
        {
            throw new InvalidOperationException($"Credit {creditId} was not found.");
        }

        var culture = CultureInfo.GetCultureInfo("es-CO");
        var clientName = WebUtility.HtmlEncode(credit.ClientName);
        var commercialName = WebUtility.HtmlEncode(credit.CommercialNameSnapshot);
        var amount = credit.Amount.ToString("C2", culture);
        var date = credit.CreatedAtUtc.ToString("yyyy-MM-dd HH:mm 'UTC'", CultureInfo.InvariantCulture);
        var subject = $"Nuevo crédito registrado - {credit.ClientName}";
        var html = $"""
            <h2>Nuevo crédito registrado</h2>
            <p><strong>Cliente:</strong> {clientName}</p>
            <p><strong>Valor del crédito:</strong> {amount}</p>
            <p><strong>Comercial:</strong> {commercialName}</p>
            <p><strong>Fecha de registro:</strong> {date}</p>
            """;
        var text = $"Cliente: {credit.ClientName}\nValor del crédito: {credit.Amount:0.00}\nComercial: {credit.CommercialNameSnapshot}\nFecha de registro: {date}";

        await emailService.SendAsync(new EmailMessage(
            notificationOptions.Value.RecipientEmail,
            subject,
            html,
            text), cancellationToken);
    }
}
