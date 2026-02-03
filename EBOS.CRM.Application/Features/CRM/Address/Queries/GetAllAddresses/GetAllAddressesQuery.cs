using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.Address.Queries.GetAllAddresses;

public record GetAllAddressQuery : IRequest<IReadOnlyCollection<AddressResponse>>;









