namespace Creditos.Api.Entities;

public sealed class Credit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string ClientName { get; set; } = string.Empty;
    public string ClientDocument { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal InterestRate { get; set; }
    public int TermMonths { get; set; }
    public Guid RegisteredByUserId { get; set; }
    public User RegisteredByUser { get; set; } = null!;
    public string CommercialNameSnapshot { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
