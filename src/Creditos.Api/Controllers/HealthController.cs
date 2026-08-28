using Creditos.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Creditos.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/health")]
public sealed class HealthController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        try
        {
            var databaseHealthy = await dbContext.Database.CanConnectAsync(cancellationToken);
            return databaseHealthy
                ? Ok(new { status = "Healthy", database = "Healthy" })
                : StatusCode(StatusCodes.Status503ServiceUnavailable, new { status = "Unhealthy", database = "Unreachable" });
        }
        catch
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { status = "Unhealthy", database = "Unreachable" });
        }
    }
}
