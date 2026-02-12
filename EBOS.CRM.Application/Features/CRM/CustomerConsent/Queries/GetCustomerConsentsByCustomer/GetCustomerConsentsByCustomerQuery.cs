using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerConsent.Queries.GetCustomerConsentsByCustomer;

public record GetCustomerConsentsByCustomerQuery(long CustomerId, int PageNumber = 1, int PageSize = 50)
    : IRequest<PagedResult<CustomerConsentResponse>>;
