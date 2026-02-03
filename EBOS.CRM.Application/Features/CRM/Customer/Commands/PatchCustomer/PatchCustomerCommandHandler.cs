using EBOS.CRM.Application.Contracts.Requests.Services;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Services.Audit;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
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
            entity.Code,
            entity.Email,
            entity.Phone,
            entity.CreatedAt,
            entity.StatusId,
            !entity.Erased);
    }
}




