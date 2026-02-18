using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
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
            ?? throw new InvalidOperationException("Parent corporate customer not found.");
        if (parent.TenantId != entityRequest.TenantId)
        {
            throw new InvalidOperationException("Parent corporate customer tenant mismatch.");
        }

        var child = await corporateCustomerRepository.GetByIdAsync(entityRequest.ChildCorporateCustomerId, cancellationToken)
            ?? throw new InvalidOperationException("Child corporate customer not found.");
        if (child.TenantId != entityRequest.TenantId)
        {
            throw new InvalidOperationException("Child corporate customer tenant mismatch.");
        }

        var entity = mapper.Map<global::EBOS.CRM.Domain.Entities.CRM.AccountHierarchy>(entityRequest);
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
        catch
        {
            await repository.RollbackAsync(cancellationToken);
            throw;
        }

        return mapper.Map<AccountHierarchyResponse>(entity);
    }
}
