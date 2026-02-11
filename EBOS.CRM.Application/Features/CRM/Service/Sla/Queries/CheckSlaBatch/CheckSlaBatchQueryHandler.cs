using EBOS.CRM.Application.Contracts.Responses.Common;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.CheckSlaBatch;

public sealed class CheckSlaBatchQueryHandler(
    ICaseRepository caseRepository,
    ISlaRepository slaRepository) : IRequestHandler<CheckSlaBatchQuery, PagedResult<SlaCheckResponse>>
{
    public async Task<PagedResult<SlaCheckResponse>> Handle(CheckSlaBatchQuery request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var payload = request.Request ?? throw new ArgumentNullException(nameof(request.Request));
        var safePageNumber = Math.Max(1, payload.PageNumber);
        var safePageSize = Math.Max(1, payload.PageSize);

        var query = caseRepository.AsQueryable()
            .Where(c => c.TenantId == payload.TenantId && c.DueAt != null && c.Status != Case.StatusClosed);

        var total = await query.CountAsync(cancellationToken);
        var cases = await query
            .OrderBy(c => c.Id)
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);

        var slaIds = cases
            .Select(c => c.SlaId)
            .Distinct()
            .ToList();

        var slas = await slaRepository.AsQueryable()
            .Where(s => slaIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        var slaMap = slas.ToDictionary(s => s.Id);
        var now = payload.Now;

        var items = cases.Select(c =>
        {
            var isActive = slaMap.TryGetValue(c.SlaId, out var sla) && sla.IsActiveAt(now);
            var isBreached = c.DueAt.HasValue && now > c.DueAt.Value;
            return new SlaCheckResponse(c.Id, c.SlaId, c.DueAt, isBreached, isActive);
        }).ToList();

        return new PagedResult<SlaCheckResponse>(items, total);
    }
}
