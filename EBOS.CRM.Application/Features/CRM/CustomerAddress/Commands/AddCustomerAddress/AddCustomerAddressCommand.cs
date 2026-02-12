using EBOS.CRM.Contracts.Requests.CRM.CustomerAddress;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerAddress.Commands.AddCustomerAddress;

public record AddCustomerAddressCommand(AddCustomerAddressRequest CustomerAddressRequest) :
    IRequest<CustomerAddressResponse>;




