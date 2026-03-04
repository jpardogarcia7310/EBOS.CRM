using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerMerge.Queries.GetCustomerMergeHistoryByMerged;

public record GetCustomerMergeHistoryByMergedQuery(long TenantId, long MergedCustomerId, int PageNumber = 1, int PageSize = 50)
    : IRequest<PagedResult<CustomerMergeHistoryResponse>>;
