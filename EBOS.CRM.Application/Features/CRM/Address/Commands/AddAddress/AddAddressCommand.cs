using EBOS.CRM.Application.Contracts.Requests.CRM;
using EBOS.CRM.Application.Contracts.Requests.CRM.Address;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Address.Commands.AddAddress;

public record AddAddressCommand(AddAddressRequest AddressRequest) : IRequest<AddressResponse>;
