using System.ComponentModel.DataAnnotations;

namespace Creditos.Api.DTOs.Credits;

public sealed class CreateCreditRequest
{
    [Required, StringLength(150, MinimumLength = 2)]
    public string ClientName { get; set; } = string.Empty;

    [Required, StringLength(50, MinimumLength = 3)]
    public string ClientDocument { get; set; } = string.Empty;

    [Required, Range(typeof(decimal), "0.01", "9999999999999999.99")]
    public decimal? Amount { get; set; }

    [Required, Range(typeof(decimal), "0", "100")]
    public decimal? InterestRate { get; set; }

    [Required, Range(1, 600)]
    public int? TermMonths { get; set; }
}
