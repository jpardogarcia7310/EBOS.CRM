using EBOS.CRM.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Commands.RouteCase;

public record RouteCaseCommand(long Id, RouteCaseRequest CaseRequest) : IRequest<CaseResponse?>;
