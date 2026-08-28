namespace Creditos.Api.Configuration;

public sealed class DemoUsersOptions
{
    public const string SectionName = "DemoUsers";
    public bool Enabled { get; set; }
    public string Password { get; set; } = string.Empty;
}
