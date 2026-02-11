using EBOS.CRM.Application.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AssignCaseSla;

public record AssignCaseSlaCommand(long Id, AssignCaseSlaRequest CaseRequest) : IRequest<CaseResponse?>;
