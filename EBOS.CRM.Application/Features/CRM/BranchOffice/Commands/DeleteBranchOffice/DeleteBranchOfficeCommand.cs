using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.DeleteBranchOffice;

public sealed record DeleteBranchOfficeCommand(long Id) : IRequest<bool>;
