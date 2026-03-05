using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Application.Features.CRM.CustomerPrivacy;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Commands.RegisterCustomerPrivacyRequest;

public sealed class RegisterCustomerPrivacyRequestCommandHandler(
    ICustomerPrivacyRequestRepository privacyRequestRepository,
    ICustomerRepository customerRepository,
    IAuditService auditService,
    ICurrentUserContext currentUser,
    CustomerPrivacyExecutionService executionService)
    : IRequestHandler<RegisterCustomerPrivacyRequestCommand, CustomerPrivacyRequestResponse>
{
    public async Task<CustomerPrivacyRequestResponse> Handle(RegisterCustomerPrivacyRequestCommand command,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var request = command.Request ?? throw new ArgumentNullException(nameof(command.Request));

        var customer = await customerRepository.GetByIdAsync(request.CustomerId, cancellationToken)
                       ?? throw new InvalidOperationException("Customer not found.");
        if (customer.TenantId != request.TenantId)
        {
            throw new InvalidOperationException("Customer tenant mismatch.");
        }

        var normalizedType = request.RequestType.Trim().ToUpperInvariant();
        var active = await privacyRequestRepository.GetActiveByCustomerAndTypeAsync(
            request.TenantId, request.CustomerId, normalizedType, cancellationToken);
        if (active is not null)
        {
            if (active.MatchesRegistrationIntent(normalizedType, request.Reason, currentUser.UserId))
            {
                return active.ToResponse();
            }

            throw new DomainConflictException(
                "An active request already exists for this customer and request type.",
                "DOMAIN_CONFLICT_PRIVACY_ACTIVE_REQUEST");
        }

        var entity = CustomerPrivacyRequest.Create(
            request.TenantId,
            request.CustomerId,
            normalizedType,
            currentUser.UserId,
            request.Reason,
            currentUser.CorrelationId);

        await privacyRequestRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            await privacyRequestRepository.AddAsync(entity, cancellationToken);
            await privacyRequestRepository.SaveChangesAsync(cancellationToken);

            await auditService.InsertAuditAsync(new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Add,
                Entity: nameof(CustomerPrivacyRequest),
                RegisterId: entity.Id,
                OldValues: null,
                NewValues: AuditSerialization.Serialize(entity),
                CorrelationId: currentUser.CorrelationId), cancellationToken);

            if (request.ExecuteNow)
            {
                await executionService.ExecuteAsync(entity, cancellationToken);
            }

            await privacyRequestRepository.CommitAsync(cancellationToken);
        }
        catch
        {
            await privacyRequestRepository.RollbackAsync(cancellationToken);
            throw;
        }

        return entity.ToResponse();
    }
}
