using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerMerge.Queries.GetCustomerMergeHistoryByWinner;

public record GetCustomerMergeHistoryByWinnerQuery(long TenantId, long WinnerCustomerId, int PageNumber = 1, int PageSize = 50)
    : IRequest<PagedResult<CustomerMergeHistoryResponse>>;
