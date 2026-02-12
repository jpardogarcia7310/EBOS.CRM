using EBOS.CRM.Contracts.Responses.EBOS;
using MediatR;

namespace EBOS.CRM.Application.Features.EBOS.Statuses.Queries.GetStatusById;

public record GetStatusByIdQuery(long Id) : IRequest<StatusResponse?>;



