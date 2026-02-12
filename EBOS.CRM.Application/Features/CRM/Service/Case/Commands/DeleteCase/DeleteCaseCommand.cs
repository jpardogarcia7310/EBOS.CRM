using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Service.Case.Commands.DeleteCase;

public record DeleteCaseCommand(long Id) : IRequest<bool>;
