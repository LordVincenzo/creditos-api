namespace Creditos.Api.Configuration;

public sealed class CreditNotificationOptions
{
    public const string SectionName = "CreditNotification";
    public string RecipientEmail { get; set; } = "creditos@gmail.com";
}
