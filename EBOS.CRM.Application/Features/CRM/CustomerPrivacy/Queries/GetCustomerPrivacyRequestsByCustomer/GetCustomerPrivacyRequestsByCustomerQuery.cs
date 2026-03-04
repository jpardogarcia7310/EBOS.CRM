using EBOS.CRM.Contracts.Responses.Common;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Queries.GetCustomerPrivacyRequestsByCustomer;

public sealed record GetCustomerPrivacyRequestsByCustomerQuery(long TenantId, long CustomerId, int PageNumber = 1,
    int PageSize = 50) : IRequest<PagedResult<CustomerPrivacyRequestResponse>>;
