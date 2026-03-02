using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM.Models;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerMerge.Queries.FindCustomerDuplicates;

public class FindCustomerDuplicatesQueryHandler(
    ICustomerDedupeRepository dedupeRepository,
    ICustomerDedupeNormalizationService normalizationService,
    ICustomer360Metrics metrics)
    : IRequestHandler<FindCustomerDuplicatesQuery, PagedResult<CustomerDuplicateCandidateResponse>>
{
    public async Task<PagedResult<CustomerDuplicateCandidateResponse>> Handle(FindCustomerDuplicatesQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var criteria = request.Request ?? throw new ArgumentNullException(nameof(request.Request));

        var dedupeCriteria = new CustomerDedupeCriteria(
            criteria.TenantId,
            normalizationService.NormalizeEmail(criteria.Email),
            normalizationService.NormalizePhone(criteria.Phone),
            normalizationService.NormalizeAlphanumericUpper(criteria.TaxId),
            normalizationService.NormalizeAlphanumericUpper(criteria.IdentificationNumber));

        var total = await dedupeRepository.CountDuplicatesAsync(dedupeCriteria, cancellationToken);
        var candidates = await dedupeRepository.FindDuplicatesAsync(dedupeCriteria, request.PageNumber, request.PageSize,
            cancellationToken);

        var items = candidates.Select(x => new CustomerDuplicateCandidateResponse(x.CustomerId, x.MatchReason, x.Score))
            .ToList();
        metrics.RecordDedupeQuery(criteria.TenantId, items.Count, items.Count == 0 ? 0 : items.Max(x => x.Score));

        return new PagedResult<CustomerDuplicateCandidateResponse>(items, total);
    }
}
