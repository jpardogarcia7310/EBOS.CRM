using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.BranchOfficeAddress.Commands.UpdateBranchOfficeAddress;

public class UpdateBranchOfficeAddressCommandHandler(IBranchOfficeAddressRepository repository,
    IAuditService auditService, ICurrentUserContext currentUser, IMapper mapper,
    IBranchOfficeAddressReferenceValidationService referenceValidationService) :
    IRequestHandler<UpdateBranchOfficeAddressCommand, BranchOfficeAddressResponse?>
{
    public async Task<BranchOfficeAddressResponse?> Handle(UpdateBranchOfficeAddressCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.BranchOfficeAddressRequest ??
                            throw new ArgumentNullException(nameof(request.BranchOfficeAddressRequest));
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return null;

        await referenceValidationService.EnsureDependenciesAvailableAsync(
            entityRequest.TenantId,
            entityRequest.BranchOfficeId,
            entityRequest.AddressId,
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
                Entity: nameof(Domain.Entities.CRM.BranchOfficeAddress),
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

        return mapper.Map<BranchOfficeAddressResponse>(entity);
    }
}




