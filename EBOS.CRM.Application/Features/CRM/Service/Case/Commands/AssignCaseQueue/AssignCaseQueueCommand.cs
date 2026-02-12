using EBOS.CRM.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AssignCaseQueue;

public record AssignCaseQueueCommand(long Id, AssignCaseQueueRequest CaseRequest) : IRequest<CaseResponse?>;
