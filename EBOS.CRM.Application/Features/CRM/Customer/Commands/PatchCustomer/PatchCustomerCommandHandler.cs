using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Customer.Commands.PatchCustomer;

public class PatchCustomerCommandHandler(ICustomerRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser) : IRequestHandler<PatchCustomerCommand, CustomerResponse?>
{
    public async Task<CustomerResponse?> Handle(PatchCustomerCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var oldValues = AuditSerialization.Serialize(entity);

        if (request.CustomerRequest.Code != null)
            entity.Code = request.CustomerRequest.Code;
        if (request.CustomerRequest.Email != null)
            entity.Email = request.CustomerRequest.Email;
        if (request.CustomerRequest.Phone != null)
            entity.Phone = request.CustomerRequest.Phone;
        if (request.CustomerRequest.StatusId.HasValue)
            entity.StatusId = request.CustomerRequest.StatusId.Value;

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Patch,
                Entity: nameof(Domain.Entities.CRM.Customer),
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

        return new CustomerResponse(
            entity.Id,
            entity.TenantId,
            entity.Code,
            entity.Email,
            entity.Phone,
            entity.StatusId,
            !entity.Erased);
    }
}




