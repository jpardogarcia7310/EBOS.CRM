using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.AccountContact.Commands.SetPrimaryAccountContact;

public class SetPrimaryAccountContactCommandHandler(
    IAccountContactRepository repository,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper)
    : IRequestHandler<SetPrimaryAccountContactCommand, AccountContactResponse?>
{
    public async Task<AccountContactResponse?> Handle(SetPrimaryAccountContactCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.AccountContactRequest ??
                            throw new ArgumentNullException(nameof(request.AccountContactRequest));

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return null;

        if (entity.TenantId != entityRequest.TenantId)
        {
            throw new InvalidOperationException("Account contact tenant mismatch.");
        }

        var oldValues = AuditSerialization.Serialize(entity);
        entity.SetPrimary(entityRequest.IsPrimary);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            if (entity.IsPrimary)
            {
                var existing = await repository.GetAllAsync(cancellationToken);
                foreach (var contact in existing.Where(x => x.CorporateCustomerId == entity.CorporateCustomerId && x.IsPrimary && x.Id != entity.Id))
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
