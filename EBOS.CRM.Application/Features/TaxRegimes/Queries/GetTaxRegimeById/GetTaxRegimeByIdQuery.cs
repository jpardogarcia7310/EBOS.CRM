using EBOS.CRM.Application.Features.TaxRegimes.Dtos;
using MediatR;

namespace EBOS.CRM.Application.Features.TaxRegimes.Queries.GetTaxRegimeById;

public record GetTaxRegimeByIdQuery(long Id) : IRequest<TaxRegimeResponseDto?>;