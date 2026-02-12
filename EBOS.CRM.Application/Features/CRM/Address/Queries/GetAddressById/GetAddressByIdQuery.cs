using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Address.Queries.GetAddressById;

public record GetAddressByIdQuery(long Id) : IRequest<AddressResponse?>;



