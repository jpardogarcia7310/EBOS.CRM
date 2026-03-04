using EBOS.CRM.Contracts.Requests.CRM.CustomerPrivacy;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Commands.RetryCustomerPrivacyRequest;

public sealed record RetryCustomerPrivacyRequestCommand(long Id, RetryCustomerPrivacyRequestRequest Request)
    : IRequest<CustomerPrivacyRequestResponse?>;
