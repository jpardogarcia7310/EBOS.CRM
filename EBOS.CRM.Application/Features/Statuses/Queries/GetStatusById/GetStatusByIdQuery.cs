using EBOS.CRM.Application.Features.Statuses.Dtos;
using MediatR;

namespace EBOS.CRM.Application.Features.Statuses.Queries.GetStatusById;

public record GetStatusByIdQuery(long Id) : IRequest<StatusResponseDto?>;