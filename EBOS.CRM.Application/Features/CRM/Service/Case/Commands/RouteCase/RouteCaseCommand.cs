using EBOS.CRM.Application.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Commands.RouteCase;

public record RouteCaseCommand(long Id, RouteCaseRequest CaseRequest) : IRequest<CaseResponse?>;
