using EBOS.CRM.Application.Contracts.Requests.CRM;
using EBOS.CRM.Application.Contracts.Requests.CRM.Address;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Address.Commands.UpdateAddress;

public record UpdateAddressCommand(long Id, UpdateAddressRequest AddressRequest) : IRequest<AddressResponse?>;
