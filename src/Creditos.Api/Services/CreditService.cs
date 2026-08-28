using Creditos.Api.Data;
using Creditos.Api.DTOs.Common;
using Creditos.Api.DTOs.Credits;
using Creditos.Api.Entities;
using Creditos.Api.Jobs;
using Microsoft.EntityFrameworkCore;

namespace Creditos.Api.Services;

public sealed class CreditService(
    AppDbContext dbContext,
    ICreditNotificationQueue notificationQueue,
    TimeProvider timeProvider) : ICreditService
{
    public async Task<CreditResponse> CreateAsync(CreateCreditRequest request, Guid authenticatedUserId, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(
            item => item.Id == authenticatedUserId && item.IsActive,
            cancellationToken);
        if (user is null)
        {
            throw new UnauthorizedAccessException("Authenticated commercial is not active or does not exist.");
        }

        var credit = new Credit
        {
            ClientName = request.ClientName.Trim(),
            ClientDocument = request.ClientDocument.Trim(),
            Amount = request.Amount ?? throw new ArgumentException("Amount is required."),
            InterestRate = request.InterestRate ?? throw new ArgumentException("InterestRate is required."),
            TermMonths = request.TermMonths ?? throw new ArgumentException("TermMonths is required."),
            RegisteredByUserId = user.Id,
            CommercialNameSnapshot = user.DisplayName,
            CreatedAtUtc = timeProvider.GetUtcNow()
        };

        dbContext.Credits.Add(credit);
        await dbContext.SaveChangesAsync(cancellationToken);
        notificationQueue.Enqueue(credit.Id);
        return Map(credit);
    }

    public async Task<PagedResult<CreditResponse>> QueryAsync(CreditQuery query, CancellationToken cancellationToken)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize <= 0 ? 20 : query.PageSize, 1, 100);
        var credits = dbContext.Credits.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.ClientName))
        {
            var value = query.ClientName.Trim().ToLowerInvariant();
            credits = credits.Where(item => item.ClientName.ToLower().Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(query.ClientDocument))
        {
            var value = query.ClientDocument.Trim().ToLowerInvariant();
            credits = credits.Where(item => item.ClientDocument.ToLower().Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(query.Commercial))
        {
            var value = query.Commercial.Trim().ToLowerInvariant();
            credits = credits.Where(item => item.CommercialNameSnapshot.ToLower().Contains(value));
        }

        var sortBy = (query.SortBy ?? "createdAt").Trim().ToLowerInvariant();
        var ascending = string.Equals(query.SortDirection ?? "desc", "asc", StringComparison.OrdinalIgnoreCase);
        credits = sortBy switch
        {
            "amount" => ascending ? credits.OrderBy(item => item.Amount).ThenBy(item => item.Id) : credits.OrderByDescending(item => item.Amount).ThenByDescending(item => item.Id),
            _ => ascending ? credits.OrderBy(item => item.CreatedAtUtc).ThenBy(item => item.Id) : credits.OrderByDescending(item => item.CreatedAtUtc).ThenByDescending(item => item.Id)
        };

        var totalItems = await credits.CountAsync(cancellationToken);
        var items = await credits
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(item => new CreditResponse(
                item.Id,
                item.ClientName,
                item.ClientDocument,
                item.Amount,
                item.InterestRate,
                item.TermMonths,
                item.RegisteredByUserId,
                item.CommercialNameSnapshot,
                item.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize);
        return new PagedResult<CreditResponse>(items, page, pageSize, totalItems, totalPages);
    }

    private static CreditResponse Map(Credit item) => new(
        item.Id,
        item.ClientName,
        item.ClientDocument,
        item.Amount,
        item.InterestRate,
        item.TermMonths,
        item.RegisteredByUserId,
        item.CommercialNameSnapshot,
        item.CreatedAtUtc);
}
