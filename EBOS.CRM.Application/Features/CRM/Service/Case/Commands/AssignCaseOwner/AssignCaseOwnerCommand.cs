using EBOS.CRM.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AssignCaseOwner;

public record AssignCaseOwnerCommand(long Id, AssignCaseOwnerRequest CaseRequest) : IRequest<CaseResponse?>;
