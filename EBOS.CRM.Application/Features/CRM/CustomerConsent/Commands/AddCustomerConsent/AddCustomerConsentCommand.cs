using EBOS.CRM.Contracts.Requests.CRM.CustomerConsent;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerConsent.Commands.AddCustomerConsent;

public record AddCustomerConsentCommand(AddCustomerConsentRequest ConsentRequest)
    : IRequest<CustomerConsentResponse>;
