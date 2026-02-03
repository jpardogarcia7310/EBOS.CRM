using EBOS.CRM.Application.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Customer.Commands.PatchCustomer;

public sealed record PatchCustomerCommand(long Id, PatchCustomerRequest CustomerRequest)
    : IRequest<CustomerResponse?>;




