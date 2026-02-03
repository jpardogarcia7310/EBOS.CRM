using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerAddress.Commands.DeleteCustomerAddress;

public record DeleteCustomerAddressCommand(long Id) : IRequest<bool>;




