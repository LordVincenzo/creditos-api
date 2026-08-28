using Creditos.Api.DTOs.Common;
using Creditos.Api.DTOs.Credits;

namespace Creditos.Api.Services;

public interface ICreditService
{
    Task<CreditResponse> CreateAsync(CreateCreditRequest request, Guid authenticatedUserId, CancellationToken cancellationToken);
    Task<PagedResult<CreditResponse>> QueryAsync(CreditQuery query, CancellationToken cancellationToken);
}
