using EBOS.CRM.Contracts.Requests.CRM.CustomerPrivacy;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Commands.ExecuteCustomerPrivacyRequest;

public sealed record ExecuteCustomerPrivacyRequestCommand(long Id, ExecuteCustomerPrivacyRequestRequest Request)
    : IRequest<CustomerPrivacyRequestResponse?>;
