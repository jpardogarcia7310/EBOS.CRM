using EBOS.CRM.Contracts.Requests.CRM.Address;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Address.Commands.UpdateAddress;

public record UpdateAddressCommand(long Id, UpdateAddressRequest AddressRequest) : IRequest<AddressResponse?>;




