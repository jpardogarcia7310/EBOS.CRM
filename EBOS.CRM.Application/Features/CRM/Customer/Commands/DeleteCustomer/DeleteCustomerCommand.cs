

using MediatR;


namespace EBOS.CRM.Application.Features.CRM.Customer.Commands.DeleteCustomer;

public record DeleteCustomerCommand(long Id) : IRequest<bool>;




