using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerMerge.Queries.GetCustomerMergeHistoryByWinner;

public class GetCustomerMergeHistoryByWinnerQueryHandler(ICustomerMergeHistoryRepository repository)
    : IRequestHandler<GetCustomerMergeHistoryByWinnerQuery, PagedResult<CustomerMergeHistoryResponse>>
{
    public async Task<PagedResult<CustomerMergeHistoryResponse>> Handle(GetCustomerMergeHistoryByWinnerQuery request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var itemsPage = await repository.GetByWinnerPagedAsync(
            request.TenantId,
            request.WinnerCustomerId,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
        var total = await repository.CountByWinnerAsync(
            request.TenantId,
            request.WinnerCustomerId,
            cancellationToken);

        var items = itemsPage
            .Select(x => new CustomerMergeHistoryResponse(
                x.Id,
                x.TenantId,
                x.WinnerCustomerId,
                x.MergedCustomerId,
                x.Reason))
            .ToList();

        return new PagedResult<CustomerMergeHistoryResponse>(items, total);
    }
}
