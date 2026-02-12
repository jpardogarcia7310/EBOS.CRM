using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Common;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerPreference.Queries.GetCustomerPreferencesByCustomer;

public record GetCustomerPreferencesByCustomerQuery(long CustomerId, int PageNumber = 1, int PageSize = 50)
    : IRequest<PagedResult<CustomerPreferenceResponse>>;
