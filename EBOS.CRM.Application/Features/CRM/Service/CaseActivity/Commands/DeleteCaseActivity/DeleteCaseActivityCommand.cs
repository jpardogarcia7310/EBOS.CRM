using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Commands.DeleteCaseActivity;

public record DeleteCaseActivityCommand(long Id) : IRequest<bool>;
