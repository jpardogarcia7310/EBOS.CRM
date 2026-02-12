using EBOS.CRM.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Commands.ReopenCase;

public record ReopenCaseCommand(long Id, ReopenCaseRequest CaseRequest) : IRequest<CaseResponse?>;
