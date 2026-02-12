using EBOS.CRM.Contracts.Responses.EBOS;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.Countries.Queries.GetCountryById;

public record GetCountryByIdQuery(long Id) : IRequest<CountryResponse?>;



