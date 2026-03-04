using EBOS.CRM.Contracts.Requests.CRM.Address;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Address.Commands.AddAddress;

public record AddAddressCommand(AddAddressRequest AddressRequest) : IRequest<AddressResponse>;




