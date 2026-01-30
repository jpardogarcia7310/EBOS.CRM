using EBOS.CRM.Application.Contracts.Requests.CRM;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Customer.Commands.UpdateCustomer;

public sealed record UpdateCustomerCommand(UpdateCustomerRequest CustomerRequest)
    : IRequest<CustomerResponse?>;
