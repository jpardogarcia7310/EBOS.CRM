using EBOS.CRM.Application.Contracts.Requests.CRM.Service.Queue;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Queue.Commands.AddQueue;

public record AddQueueCommand(AddQueueRequest QueueRequest) : IRequest<QueueResponse>;
