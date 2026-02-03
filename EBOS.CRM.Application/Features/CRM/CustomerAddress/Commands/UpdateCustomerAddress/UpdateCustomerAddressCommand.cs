using EBOS.CRM.Application.Contracts.Requests.CRM.CustomerAddress;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerAddress.Commands.UpdateCustomerAddress;

public record UpdateCustomerAddressCommand(long Id, UpdateCustomerAddressRequest CustomerAddressRequest) : IRequest<CustomerAddressResponse?>;




