using EBOS.CRM.Application.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Commands.UpdateCase;

public record UpdateCaseCommand(long Id, UpdateCaseRequest CaseRequest) : IRequest<CaseResponse?>;
