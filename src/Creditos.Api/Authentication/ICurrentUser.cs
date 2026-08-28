namespace Creditos.Api.Authentication;

public interface ICurrentUser
{
    Guid? UserId { get; }
    string? Email { get; }
    string? DisplayName { get; }
}
