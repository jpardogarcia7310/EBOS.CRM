using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerPreference.Commands.UpsertCustomerPreference;

public class UpsertCustomerPreferenceCommandHandler(
    ICustomerPreferenceRepository repository,
    ICustomerRepository customerRepository,
    IChannelTypeRepository channelTypeRepository,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    IMapper mapper)
    : IRequestHandler<UpsertCustomerPreferenceCommand, CustomerPreferenceResponse>
{
    public async Task<CustomerPreferenceResponse> Handle(UpsertCustomerPreferenceCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.PreferenceRequest ??
                            throw new ArgumentNullException(nameof(request.PreferenceRequest));

        var customer = await customerRepository.GetByIdAsync(entityRequest.CustomerId, cancellationToken)
            ?? throw new DomainValidationException("Customer not found.", "DOMAIN_VALIDATION_CUSTOMER_NOT_FOUND");
        if (customer.TenantId != entityRequest.TenantId)
        {
            throw new DomainConflictException("Customer tenant mismatch.", "DOMAIN_CONFLICT_CUSTOMER_TENANT_MISMATCH");
        }

        var channelType = await channelTypeRepository.GetByIdAsync(entityRequest.ChannelId, cancellationToken)
            ?? throw new DomainValidationException("Channel type not found.", "DOMAIN_VALIDATION_CHANNEL_TYPE_NOT_FOUND");
        if (!channelType.IsActive)
        {
            throw new DomainRuleViolationException("Channel type is not active.", "DOMAIN_RULE_VIOLATION_CHANNEL_TYPE_INACTIVE");
        }

        var existing = await repository.GetByCustomerAndChannelAsync(
            entityRequest.TenantId,
            entityRequest.CustomerId,
            entityRequest.ChannelId,
            cancellationToken);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            if (existing is null)
            {
                var entity = global::EBOS.CRM.Domain.Entities.CRM.CustomerPreference.Create(
                    entityRequest.TenantId,
                    entityRequest.CustomerId,
                    entityRequest.ChannelId,
                    entityRequest.Preferred,
                    DateTime.UtcNow,
                    currentUser.UserId);

                await repository.AddAsync(entity, cancellationToken);
                await repository.SaveChangesAsync(cancellationToken);

                var auditRequest = new AuditInsertRequest(
                    UserId: currentUser.UserId,
                    TimeStamp: DateTimeOffset.UtcNow,
                    Action: AuditActions.Add,
                    Entity: nameof(Domain.Entities.CRM.CustomerPreference),
                    RegisterId: entity.Id,
                    OldValues: null,
                    NewValues: AuditSerialization.Serialize(entity),
                    CorrelationId: currentUser.CorrelationId);

                await auditService.InsertAuditAsync(auditRequest, cancellationToken);
                await repository.CommitAsync(cancellationToken);

                return mapper.Map<CustomerPreferenceResponse>(entity);
            }

            var oldValues = AuditSerialization.Serialize(existing);
            existing.UpdatePreference(entityRequest.Preferred, DateTime.UtcNow, currentUser.UserId);

            await repository.UpdateAsync(existing, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var updateAuditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Update,
                Entity: nameof(Domain.Entities.CRM.CustomerPreference),
                RegisterId: existing.Id,
                OldValues: oldValues,
                NewValues: AuditSerialization.Serialize(existing),
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(updateAuditRequest, cancellationToken);
            await repository.CommitAsync(cancellationToken);

            return mapper.Map<CustomerPreferenceResponse>(existing);
        }
        catch
        {
            await repository.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
