using EBOS.CRM.Contracts.Requests.CRM.CustomerPrivacy;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Commands.RegisterCustomerPrivacyRequest;

public sealed record RegisterCustomerPrivacyRequestCommand(RegisterCustomerPrivacyRequestRequest Request)
    : IRequest<CustomerPrivacyRequestResponse>;
