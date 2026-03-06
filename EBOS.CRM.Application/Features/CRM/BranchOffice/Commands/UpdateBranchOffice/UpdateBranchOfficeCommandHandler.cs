using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BranchOffice.Commands.UpdateBranchOffice;

public class UpdateBranchOfficeCommandHandler(IBranchOfficeRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser, IMapper mapper, IBranchOfficeReferenceValidationService referenceValidationService)
    : IRequestHandler<UpdateBranchOfficeCommand, BranchOfficeResponse?>
{
    public async Task<BranchOfficeResponse?> Handle(UpdateBranchOfficeCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.BranchOfficeRequest ??
                            throw new ArgumentNullException(nameof(request.BranchOfficeRequest));
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return null;
        await referenceValidationService.EnsureCorporateCustomerAvailableAsync(
            entityRequest.TenantId,
            entityRequest.CorporateCustomerId,
            cancellationToken);

        var oldValues = AuditSerialization.Serialize(entity);
        mapper.Map(entityRequest, entity);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Update,
                Entity: nameof(global::EBOS.CRM.Domain.Entities.CRM.BranchOffice),
                RegisterId: entity.Id,
                OldValues: oldValues,
                NewValues: AuditSerialization.Serialize(entity),
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(auditRequest, cancellationToken);
            await repository.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await repository.RollbackAsync(cancellationToken);

            if (DomainTransientFailureClassifier.TryClassify(ex, nameof(Handle), out var transient))
            {
                throw transient;
            }

            throw;
        }

        return mapper.Map<BranchOfficeResponse>(entity);
    }
}





