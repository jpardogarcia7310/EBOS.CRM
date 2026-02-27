using EBOS.CRM.Contracts.Requests.CRM.CustomerConsent;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerConsent.Commands.RevokeCustomerConsent;

public record RevokeCustomerConsentCommand(long Id, RevokeCustomerConsentRequest ConsentRequest)
    : IRequest<CustomerConsentResponse?>;
