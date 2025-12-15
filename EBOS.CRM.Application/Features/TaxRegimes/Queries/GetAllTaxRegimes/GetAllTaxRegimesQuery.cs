using EBOS.CRM.Application.Features.TaxRegimes.Dtos;
using MediatR;

namespace EBOS.CRM.Application.Features.TaxRegimes.Queries.GetAllTaxRegimes;

public record GetAllTaxRegimesQuery() : IRequest<IEnumerable<TaxRegimeResponseDto>>;