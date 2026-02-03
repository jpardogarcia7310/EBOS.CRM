using EBOS.CRM.Application.Contracts.Responses;
using MediatR;


namespace EBOS.CRM.Application.Features.AddressesType.Query.GetAllAddressesType;

public record GetAllAddressesTypeQuery : IRequest<IReadOnlyCollection<AddressTypeResponse>>;










