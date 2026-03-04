using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Queries.GetCustomerPrivacyRequestById;

public sealed record GetCustomerPrivacyRequestByIdQuery(long Id, long TenantId)
    : IRequest<CustomerPrivacyRequestResponse?>;
