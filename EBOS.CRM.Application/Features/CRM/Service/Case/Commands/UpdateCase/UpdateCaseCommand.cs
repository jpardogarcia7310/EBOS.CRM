using EBOS.CRM.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Commands.UpdateCase;

public record UpdateCaseCommand(long Id, UpdateCaseRequest CaseRequest) : IRequest<CaseResponse?>;
