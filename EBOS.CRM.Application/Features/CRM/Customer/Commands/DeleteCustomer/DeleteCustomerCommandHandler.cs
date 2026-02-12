using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Customer.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandler(ICustomerRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser) : IRequestHandler<DeleteCustomerCommand, bool>
{
    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return false;

        var oldValues = AuditSerialization.Serialize(entity);
        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.DeleteAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Delete,
                Entity: nameof(Domain.Entities.CRM.Customer),
                RegisterId: entity.Id,
                OldValues: oldValues,
                NewValues: null,
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(auditRequest, cancellationToken);
            await repository.CommitAsync(cancellationToken);
        }
        catch
        {
            await repository.RollbackAsync(cancellationToken);
            throw;
        }

        return true;
    }
}




