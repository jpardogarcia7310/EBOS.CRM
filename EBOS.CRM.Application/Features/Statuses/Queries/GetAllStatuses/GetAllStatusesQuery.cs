using EBOS.CRM.Application.Features.Statuses.Dtos;
using MediatR;

namespace EBOS.CRM.Application.Features.Statuses.Queries.GetAllStatuses;

public record GetAllStatusesQuery : IRequest<IEnumerable<StatusResponseDto>>;