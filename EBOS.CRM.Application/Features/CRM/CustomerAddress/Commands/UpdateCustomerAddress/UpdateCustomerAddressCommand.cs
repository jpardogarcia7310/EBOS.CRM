using EBOS.CRM.Contracts.Requests.CRM.CustomerAddress;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerAddress.Commands.UpdateCustomerAddress;

public record UpdateCustomerAddressCommand(long Id, UpdateCustomerAddressRequest CustomerAddressRequest) :
    IRequest<CustomerAddressResponse?>;




