using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountHierarchy.Commands.EndAccountHierarchy;

public class EndAccountHierarchyCommandHandler(
    IAccountHierarchyRepository repository,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper)
    : IRequestHandler<EndAccountHierarchyCommand, AccountHierarchyResponse?>
{
    public async Task<AccountHierarchyResponse?> Handle(EndAccountHierarchyCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.AccountHierarchyRequest ??
                            throw new ArgumentNullException(nameof(request.AccountHierarchyRequest));

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return null;

        if (entity.TenantId != entityRequest.TenantId)
        {
            throw new DomainConflictException("Account hierarchy tenant mismatch.", "DOMAIN_CONFLICT_ACCOUNT_HIERARCHY_TENANT_MISMATCH");
        }

        var oldValues = AuditSerialization.Serialize(entity);
        entity.EndRelation(entityRequest.ValidTo);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Update,
                Entity: nameof(Domain.Entities.CRM.AccountHierarchy),
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

        return mapper.Map<AccountHierarchyResponse>(entity);
    }
}
