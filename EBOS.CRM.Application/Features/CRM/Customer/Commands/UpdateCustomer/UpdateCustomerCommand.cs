using EBOS.CRM.Application.Contracts.Requests.CRM.Customer;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.Customer.Commands.UpdateCustomer;

public record UpdateCustomerCommand(long Id, UpdateCustomerRequest CustomerRequest) : IRequest<CustomerResponse?>;




