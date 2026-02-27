using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerConsent.Commands.RevokeCustomerConsent;

public class RevokeCustomerConsentCommandHandler(
    ICustomerConsentRepository repository,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper)
    : IRequestHandler<RevokeCustomerConsentCommand, CustomerConsentResponse?>
{
    public async Task<CustomerConsentResponse?> Handle(RevokeCustomerConsentCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.ConsentRequest ??
                            throw new ArgumentNullException(nameof(request.ConsentRequest));

        var existing = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (existing is null)
            return null;

        if (existing.TenantId != entityRequest.TenantId)
        {
            throw new InvalidOperationException("Customer consent tenant mismatch.");
        }

        var newEvent = global::EBOS.CRM.Domain.Entities.CRM.CustomerConsent.CreateRevoked(
            existing.TenantId,
            existing.CustomerId,
            existing.ConsentType,
            entityRequest.RevokedAt,
            existing.Source,
            existing.ExpiresAt);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.AddAsync(newEvent, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Add,
                Entity: nameof(Domain.Entities.CRM.CustomerConsent),
                RegisterId: newEvent.Id,
                OldValues: null,
                NewValues: AuditSerialization.Serialize(newEvent),
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(auditRequest, cancellationToken);
            await repository.CommitAsync(cancellationToken);
        }
        catch
        {
            await repository.RollbackAsync(cancellationToken);
            throw;
        }

        return mapper.Map<CustomerConsentResponse>(newEvent);
    }
}
