using Creditos.Api.Authentication;
using Creditos.Api.DTOs.Common;
using Creditos.Api.DTOs.Credits;
using Creditos.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Creditos.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/credits")]
public sealed class CreditsController(ICreditService creditService, ICurrentUser currentUser) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("credit-create")]
    [ProducesResponseType<CreditResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CreditResponse>> Create(CreateCreditRequest request, CancellationToken cancellationToken)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Unauthorized();
        }

        var result = await creditService.CreateAsync(request, userId, cancellationToken);
        return Created("/api/credits", result);
    }

    [HttpGet]
    [ProducesResponseType<PagedResult<CreditResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<CreditResponse>>> Query([FromQuery] CreditQuery query, CancellationToken cancellationToken)
    {
        return Ok(await creditService.QueryAsync(query, cancellationToken));
    }
}
