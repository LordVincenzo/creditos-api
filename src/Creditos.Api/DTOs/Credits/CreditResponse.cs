namespace Creditos.Api.DTOs.Credits;

public sealed record CreditResponse(
    Guid Id,
    string ClientName,
    string ClientDocument,
    decimal Amount,
    decimal InterestRate,
    int TermMonths,
    Guid RegisteredByUserId,
    string CommercialName,
    DateTimeOffset CreatedAtUtc);
