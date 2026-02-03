using EBOS.CRM.Application.Contracts.Requests.CRM.CustomerAddress;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerAddress.Commands.AddCustomerAddress;

public record AddCustomerAddressCommand(AddCustomerAddressRequest CustomerAddressRequest) : IRequest<CustomerAddressResponse>;




