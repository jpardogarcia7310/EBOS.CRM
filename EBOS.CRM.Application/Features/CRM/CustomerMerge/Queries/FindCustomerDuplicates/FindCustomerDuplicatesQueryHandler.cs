using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM.Models;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerMerge.Queries.FindCustomerDuplicates;

public class FindCustomerDuplicatesQueryHandler(ICustomerDedupeRepository dedupeRepository)
    : IRequestHandler<FindCustomerDuplicatesQuery, PagedResult<CustomerDuplicateCandidateResponse>>
{
    public async Task<PagedResult<CustomerDuplicateCandidateResponse>> Handle(FindCustomerDuplicatesQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var criteria = request.Request ?? throw new ArgumentNullException(nameof(request.Request));

        var dedupeCriteria = new CustomerDedupeCriteria(
            criteria.TenantId,
            NormalizeEmail(criteria.Email),
            NormalizePhone(criteria.Phone),
            NormalizeAlphanumericUpper(criteria.TaxId),
            NormalizeAlphanumericUpper(criteria.IdentificationNumber));

        var total = await dedupeRepository.CountDuplicatesAsync(dedupeCriteria, cancellationToken);
        var candidates = await dedupeRepository.FindDuplicatesAsync(dedupeCriteria, request.PageNumber, request.PageSize,
            cancellationToken);

        var items = candidates.Select(x => new CustomerDuplicateCandidateResponse(x.CustomerId, x.MatchReason, x.Score))
            .ToList();

        return new PagedResult<CustomerDuplicateCandidateResponse>(items, total);
    }

    private static string? NormalizeEmail(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim().ToLowerInvariant();

    private static string? NormalizePhone(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var digits = new string(value.Where(char.IsDigit).ToArray());
        return string.IsNullOrWhiteSpace(digits) ? null : digits;
    }

    private static string? NormalizeAlphanumericUpper(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var filtered = new string(value.Where(char.IsLetterOrDigit).ToArray());
        return string.IsNullOrWhiteSpace(filtered) ? null : filtered.ToUpperInvariant();
    }
}
