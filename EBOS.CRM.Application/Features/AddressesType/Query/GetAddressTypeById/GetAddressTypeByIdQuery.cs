using EBOS.CRM.Application.Contracts.Responses;
using MediatR;


namespace EBOS.CRM.Application.Features.AddressesType.Query.GetAddressTypeById;

public record GetAddressTypeByIdQuery(long Id) : IRequest<AddressTypeResponse?>;



