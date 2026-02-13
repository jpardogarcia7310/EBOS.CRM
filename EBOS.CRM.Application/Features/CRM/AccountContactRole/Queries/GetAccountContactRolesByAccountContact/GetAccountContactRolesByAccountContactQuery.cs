using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContactRole.Queries.GetAccountContactRolesByAccountContact;

public record GetAccountContactRolesByAccountContactQuery(long TenantId, long AccountContactId, int PageNumber = 1, int PageSize = 50)
    : IRequest<PagedResult<AccountContactRoleResponse>>;
