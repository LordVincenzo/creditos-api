using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Creditos.Api.Authentication;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;

    public Guid? UserId => Guid.TryParse(Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub), out var id) ? id : null;
    public string? Email => Principal?.FindFirstValue(JwtRegisteredClaimNames.Email);
    public string? DisplayName => Principal?.FindFirstValue("display_name");
}
