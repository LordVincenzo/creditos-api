using System.ComponentModel.DataAnnotations;

namespace Creditos.Api.DTOs.Auth;

public sealed class LoginRequest
{
    [Required, EmailAddress, MaxLength(320)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8), MaxLength(200)]
    public string Password { get; set; } = string.Empty;
}
