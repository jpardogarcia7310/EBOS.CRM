using EBOS.CRM.Contracts.Requests.CRM.Service.Queue;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Queue.Commands.AddQueue;

public record AddQueueCommand(AddQueueRequest QueueRequest) : IRequest<QueueResponse>;
