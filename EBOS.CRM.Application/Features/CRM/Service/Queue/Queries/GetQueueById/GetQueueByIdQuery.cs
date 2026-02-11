using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Queue.Queries.GetQueueById;

public record GetQueueByIdQuery(long Id) : IRequest<QueueResponse?>;
