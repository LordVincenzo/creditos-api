namespace Creditos.Api.DTOs.Credits;

public sealed class CreditQuery
{
    public string? ClientName { get; set; }
    public string? ClientDocument { get; set; }
    public string? Commercial { get; set; }
    public string SortBy { get; set; } = "createdAt";
    public string SortDirection { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
