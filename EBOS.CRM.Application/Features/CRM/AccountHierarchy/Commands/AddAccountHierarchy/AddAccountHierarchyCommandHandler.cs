using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountHierarchy.Commands.AddAccountHierarchy;

public class AddAccountHierarchyCommandHandler(IAccountHierarchyRepository repository,
    ICorporateCustomerRepository corporateCustomerRepository, IAccountHierarchyAcyclicInvariant hierarchyInvariant,
    IAuditService auditService, ICurrentUserContext currentUser, IMapper mapper) : 
    IRequestHandler<AddAccountHierarchyCommand, AccountHierarchyResponse>
{
    public async Task<AccountHierarchyResponse> Handle(AddAccountHierarchyCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.AccountHierarchyRequest ??
                            throw new ArgumentNullException(nameof(request.AccountHierarchyRequest));

        var parent = await corporateCustomerRepository.GetByIdAsync(entityRequest.ParentCorporateCustomerId, cancellationToken)
            ?? throw new DomainValidationException("Parent corporate customer not found.", "DOMAIN_VALIDATION_PARENT_CORPORATE_CUSTOMER_NOT_FOUND");
        if (parent.TenantId != entityRequest.TenantId)
        {
            throw new DomainConflictException("Parent corporate customer tenant mismatch.", "DOMAIN_CONFLICT_PARENT_CORPORATE_CUSTOMER_TENANT_MISMATCH");
        }

        var child = await corporateCustomerRepository.GetByIdAsync(entityRequest.ChildCorporateCustomerId, cancellationToken)
            ?? throw new DomainValidationException("Child corporate customer not found.", "DOMAIN_VALIDATION_CHILD_CORPORATE_CUSTOMER_NOT_FOUND");
        if (child.TenantId != entityRequest.TenantId)
        {
            throw new DomainConflictException("Child corporate customer tenant mismatch.", "DOMAIN_CONFLICT_CHILD_CORPORATE_CUSTOMER_TENANT_MISMATCH");
        }

        var entity = global::EBOS.CRM.Domain.Entities.CRM.AccountHierarchy.Create(
            entityRequest.TenantId,
            entityRequest.ParentCorporateCustomerId,
            entityRequest.ChildCorporateCustomerId,
            entityRequest.RelationType,
            entityRequest.ValidFrom);
        await entity.AssignParentAsync(entityRequest.TenantId, entityRequest.ParentCorporateCustomerId,
            entityRequest.ChildCorporateCustomerId, entityRequest.RelationType, entityRequest.ValidFrom,
            hierarchyInvariant, cancellationToken);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.AddAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Add,
                Entity: nameof(Domain.Entities.CRM.AccountHierarchy),
                RegisterId: entity.Id,
                OldValues: null,
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

        return mapper.Map<AccountHierarchyResponse>(entity);
    }
}

