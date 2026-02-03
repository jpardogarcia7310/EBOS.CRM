using EBOS.CRM.Application.Contracts.Requests.Services;
using EBOS.CRM.Application.Services.Audit;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MediatR;


namespace EBOS.CRM.Application.Features.CRM.CustomerAddress.Commands.DeleteCustomerAddress;

public class DeleteCustomerAddressCommandHandler(ICustomerAddressRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser) : IRequestHandler<DeleteCustomerAddressCommand, bool>
{
    public async Task<bool> Handle(DeleteCustomerAddressCommand request, CancellationToken cancellationToken)
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
                Entity: nameof(Domain.Entities.CRM.CustomerAddress),
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




