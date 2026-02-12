using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MediatR;
using CaseEntity = EBOS.CRM.Domain.Entities.CRM.Case;

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

        var casesAll = await caseRepository.GetAllAsync(cancellationToken);
        var filteredCases = casesAll
            .Where(c => c.TenantId == payload.TenantId && c.DueAt != null && c.Status != CaseEntity.StatusClosed)
            .OrderBy(c => c.Id)
            .ToList();

        var total = filteredCases.Count;
        var cases = filteredCases
            .Skip((safePageNumber - 1) * safePageSize)
            .Take(safePageSize)
            .ToList();

        var slaIds = cases
            .Select(c => c.SlaId)
            .Distinct()
            .ToList();

        var slasAll = await slaRepository.GetAllAsync(cancellationToken);
        var slas = slasAll
            .Where(s => slaIds.Contains(s.Id))
            .ToList();

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
