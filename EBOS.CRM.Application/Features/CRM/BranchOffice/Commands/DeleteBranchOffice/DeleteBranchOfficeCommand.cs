using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.DeleteBranchOffice;

public record DeleteBranchOfficeCommand(long Id) : IRequest<bool>;
