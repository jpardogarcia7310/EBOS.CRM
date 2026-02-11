using EBOS.CRM.Application.Contracts.Responses.EBOS;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.AddressesType.Query.GetAddressTypeById;

public record GetAddressTypeByIdQuery(long Id) : IRequest<AddressTypeResponse?>;



