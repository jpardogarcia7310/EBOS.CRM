using EBOS.CRM.Contracts.Requests.CRM.Service.Queue;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Queue.Commands.ToggleQueue;

public record ToggleQueueCommand(long Id, ToggleQueueRequest QueueRequest) : IRequest<QueueResponse?>;
