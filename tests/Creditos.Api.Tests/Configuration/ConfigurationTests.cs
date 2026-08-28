using Creditos.Api.Configuration;

namespace Creditos.Api.Tests.Configuration;

public class ConfigurationTests
{
    [Fact]
    public void JwtKeyMustBeAtLeast32Characters()
    {
        var options = new JwtOptions { Issuer = "creditos-api", Audience = "creditos-mobile", Key = "short", ExpirationMinutes = 60 };
        Assert.False(AppOptionsValidator.ValidateJwt(options, out _));
    }

    [Fact]
    public void SmtpPortMustBeValid()
    {
        var options = new SmtpOptions { Host = "smtp.example.test", Port = 70000, FromEmail = "noreply@example.test", FromName = "Creditos" };
        Assert.False(AppOptionsValidator.ValidateSmtp(options, out _));
    }
}
