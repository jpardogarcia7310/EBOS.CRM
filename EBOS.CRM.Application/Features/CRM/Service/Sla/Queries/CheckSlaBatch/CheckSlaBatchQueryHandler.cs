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

        var cases = (await caseRepository.GetOpenSlaBatchAsync(payload.TenantId, safePageNumber, safePageSize, cancellationToken))
            .ToList();
        var total = await caseRepository.CountOpenSlaBatchAsync(payload.TenantId, cancellationToken);

        var slaIds = cases
            .Select(c => c.SlaId)
            .Distinct()
            .ToList();

        var slas = (await slaRepository.GetByIdsAsync(slaIds, cancellationToken))
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
