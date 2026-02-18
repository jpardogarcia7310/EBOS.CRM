using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Commands.UpdateAccountContact;

public class UpdateAccountContactCommandHandler(
    IAccountContactRepository repository,
    ICorporateCustomerRepository corporateCustomerRepository,
    IIndividualCustomerRepository individualCustomerRepository,
    IAccountContactPrimaryGuard primaryGuard,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper)
    : IRequestHandler<UpdateAccountContactCommand, AccountContactResponse?>
{
    public async Task<AccountContactResponse?> Handle(UpdateAccountContactCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.AccountContactRequest ??
                            throw new ArgumentNullException(nameof(request.AccountContactRequest));

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return null;

        var corporateCustomer = await corporateCustomerRepository.GetByIdAsync(entityRequest.CorporateCustomerId, cancellationToken)
            ?? throw new InvalidOperationException("Corporate customer not found.");
        if (corporateCustomer.TenantId != entityRequest.TenantId)
        {
            throw new InvalidOperationException("Corporate customer tenant mismatch.");
        }

        var individualCustomer = await individualCustomerRepository.GetByIdAsync(entityRequest.IndividualCustomerId, cancellationToken)
            ?? throw new InvalidOperationException("Individual customer not found.");
        if (individualCustomer.TenantId != entityRequest.TenantId)
        {
            throw new InvalidOperationException("Individual customer tenant mismatch.");
        }

        var oldValues = AuditSerialization.Serialize(entity);
        mapper.Map(entityRequest, entity);
        entity.Assign(entityRequest.CorporateCustomerId, entityRequest.IndividualCustomerId, entityRequest.StartAt);
        if (entityRequest.EndAt.HasValue)
        {
            entity.Unassign(entityRequest.EndAt.Value);
        }
        entity.SetPrimary(entityRequest.IsPrimary);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            if (entity.IsPrimary)
            {
                var existing = await primaryGuard.GetOtherPrimariesAsync(entityRequest.TenantId,
                    entity.CorporateCustomerId, entity.Id, cancellationToken);
                foreach (var contact in existing)
                {
                    contact.SetPrimary(false);
                    await repository.UpdateAsync(contact, cancellationToken);
                }
            }

            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Update,
                Entity: nameof(Domain.Entities.CRM.AccountContact),
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

        return mapper.Map<AccountContactResponse>(entity);
    }
}
