using EBOS.CRM.Application.Contracts.Requests.CRM.Service.Queue;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Queue.Commands.UpdateQueue;

public record UpdateQueueCommand(long Id, UpdateQueueRequest QueueRequest) : IRequest<QueueResponse?>;
