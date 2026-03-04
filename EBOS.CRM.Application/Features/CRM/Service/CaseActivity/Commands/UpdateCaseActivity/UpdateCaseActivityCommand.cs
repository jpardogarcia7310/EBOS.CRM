using EBOS.CRM.Contracts.Requests.CRM.Service.CaseActivity;
using EBOS.CRM.Contracts.Responses.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Commands.UpdateCaseActivity;

public record UpdateCaseActivityCommand(long Id, UpdateCaseActivityRequest ActivityRequest)
    : IRequest<CaseActivityResponse?>;
