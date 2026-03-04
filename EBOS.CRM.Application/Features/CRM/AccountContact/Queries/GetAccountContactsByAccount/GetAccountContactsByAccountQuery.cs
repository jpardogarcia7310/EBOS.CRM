using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Queries.GetAccountContactsByAccount;

public record GetAccountContactsByAccountQuery(long TenantId, long CorporateCustomerId, int PageNumber = 1, int PageSize = 50)
    : IRequest<PagedResult<AccountContactResponse>>;
