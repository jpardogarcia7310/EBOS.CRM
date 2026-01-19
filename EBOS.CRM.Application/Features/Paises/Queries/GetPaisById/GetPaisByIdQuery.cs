using EBOS.CRM.Application.Features.Countries.Dtos;
using MediatR;

namespace EBOS.CRM.Application.Features.Countries.Queries.GetCountryById;

public record GetPaisByIdQuery(long Id) : IRequest<PaisResponseDto?>;