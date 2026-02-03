using EBOS.CRM.Application.Contracts.Responses;
using MediatR;


namespace EBOS.CRM.Application.Features.Statuses.Queries.GetStatusById;

public record GetStatusByIdQuery(long Id) : IRequest<StatusResponse?>;



