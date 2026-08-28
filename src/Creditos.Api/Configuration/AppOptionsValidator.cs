namespace Creditos.Api.Configuration;

public static class AppOptionsValidator
{
    public static bool ValidateJwt(JwtOptions options, out string error)
    {
        if (string.IsNullOrWhiteSpace(options.Issuer) || string.IsNullOrWhiteSpace(options.Audience))
        {
            error = "JWT issuer and audience are required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(options.Key) || options.Key.Length < 32)
        {
            error = "JWT key must contain at least 32 characters.";
            return false;
        }

        if (options.ExpirationMinutes is < 5 or > 1440)
        {
            error = "JWT expiration must be between 5 and 1440 minutes.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    public static bool ValidateSmtp(SmtpOptions options, out string error)
    {
        if (string.IsNullOrWhiteSpace(options.Host))
        {
            error = "SMTP host is required.";
            return false;
        }

        if (options.Port is < 1 or > 65535)
        {
            error = "SMTP port is invalid.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(options.FromEmail))
        {
            error = "SMTP from email is required.";
            return false;
        }

        error = string.Empty;
        return true;
    }
}
