using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerConsent.Commands.AddCustomerConsent;

public class AddCustomerConsentCommandHandler(
    ICustomerConsentRepository repository,
    ICustomerRepository customerRepository,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper)
    : IRequestHandler<AddCustomerConsentCommand, CustomerConsentResponse>
{
    public async Task<CustomerConsentResponse> Handle(AddCustomerConsentCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.ConsentRequest ??
                            throw new ArgumentNullException(nameof(request.ConsentRequest));

        var customer = await customerRepository.GetByIdAsync(entityRequest.CustomerId, cancellationToken)
            ?? throw new InvalidOperationException("Customer not found.");
        if (customer.TenantId != entityRequest.TenantId)
        {
            throw new InvalidOperationException("Customer tenant mismatch.");
        }

        var entity = global::EBOS.CRM.Domain.Entities.CRM.CustomerConsent.Create(
            entityRequest.TenantId,
            entityRequest.CustomerId,
            entityRequest.ConsentType,
            entityRequest.Granted,
            entityRequest.GrantedAt,
            entityRequest.Source,
            entityRequest.ExpiresAt);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.AddAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Add,
                Entity: nameof(Domain.Entities.CRM.CustomerConsent),
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

        return mapper.Map<CustomerConsentResponse>(entity);
    }
}
