using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerMerge.Queries.GetCustomerMergeHistoryByMerged;

public class GetCustomerMergeHistoryByMergedQueryHandler(ICustomerMergeHistoryRepository repository)
    : IRequestHandler<GetCustomerMergeHistoryByMergedQuery, PagedResult<CustomerMergeHistoryResponse>>
{
    public async Task<PagedResult<CustomerMergeHistoryResponse>> Handle(GetCustomerMergeHistoryByMergedQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var itemsPage = await repository.GetByMergedPagedAsync(
            request.TenantId,
            request.MergedCustomerId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
        var total = await repository.CountByMergedAsync(
            request.TenantId,
            request.MergedCustomerId,
            cancellationToken);

        var items = itemsPage
            .Select(x => new CustomerMergeHistoryResponse(
                x.Id,
                x.TenantId,
                x.WinnerCustomerId,
                x.MergedCustomerId,
                x.Reason,
                x.CreatedBy,
                x.CreatedAt))
            .ToList();

        return new PagedResult<CustomerMergeHistoryResponse>(items, total);
    }
}
