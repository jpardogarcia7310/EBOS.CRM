using EBOS.CRM.Application.Contracts.Responses;
using MediatR;

namespace EBOS.CRM.Application.Features.Statuses.Queries.GetAllStatuses;

public record GetAllStatusesQuery : IRequest<IEnumerable<StatusResponse>>;