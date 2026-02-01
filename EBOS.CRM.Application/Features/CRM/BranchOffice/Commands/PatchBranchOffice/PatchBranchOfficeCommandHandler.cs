using EBOS.CRM.Application.Contracts.Requests.Services;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Services.Audit;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.PatchBranchOffice;

public class PatchBranchOfficeCommandHandler(IBranchOfficeRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser) : IRequestHandler<PatchBranchOfficeCommand, BranchOfficeResponse?>
{
    public async Task<BranchOfficeResponse?> Handle(PatchBranchOfficeCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var oldValues = AuditSerialization.Serialize(entity);

        if (request.BranchOfficeRequest.Name != null)
            entity.Name = request.BranchOfficeRequest.Name;
        if (request.BranchOfficeRequest.PhoneNumber != null)
            entity.PhoneNumber = request.BranchOfficeRequest.PhoneNumber;
        if (request.BranchOfficeRequest.CorporateCustomerId.HasValue)
            entity.CorporateCustomerId = request.BranchOfficeRequest.CorporateCustomerId.Value;

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Patch,
                Entity: nameof(Domain.Entities.CRM.BranchOffice),
                RegisterId: entity.Id,
                OldValues: oldValues,
                NewValues: AuditSerialization.Serialize(entity),
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(auditRequest, cancellationToken);

            await repository.CommitAsync(cancellationToken);
        }
        catch
        {
            await repository.RollbackAsync(cancellationToken);
            throw;
        }

        return new BranchOfficeResponse(
            entity.Id,
            entity.Name,
            entity.PhoneNumber,
            entity.CorporateCustomerId,
            !entity.Erased);
    }
}
