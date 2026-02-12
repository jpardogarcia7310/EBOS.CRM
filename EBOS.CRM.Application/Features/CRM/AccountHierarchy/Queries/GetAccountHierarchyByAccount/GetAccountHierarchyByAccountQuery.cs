using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountHierarchy.Queries.GetAccountHierarchyByAccount;

public record GetAccountHierarchyByAccountQuery(long CorporateCustomerId, int PageNumber = 1, int PageSize = 50)
    : IRequest<PagedResult<AccountHierarchyResponse>>;
