using EBOS.CRM.Application.Features.CRM.CustomerPrivacy;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Application.Shared.Observability;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Commands.RetryCustomerPrivacyRequest;

public sealed class RetryCustomerPrivacyRequestCommandHandler(
    ICustomerPrivacyRequestRepository privacyRequestRepository,
    CustomerPrivacyExecutionService executionService,
    ICurrentUserContext currentUser,
    IDomainOperationalEventPublisher domainOperationalEventPublisher)
    : IRequestHandler<RetryCustomerPrivacyRequestCommand, CustomerPrivacyRequestResponse?>
{
    public async Task<CustomerPrivacyRequestResponse?> Handle(RetryCustomerPrivacyRequestCommand command,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var request = command.Request ?? throw new ArgumentNullException(nameof(command.Request));

        var entity = await privacyRequestRepository.GetByIdAsync(command.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        if (entity.TenantId != request.TenantId)
        {
            throw new InvalidOperationException("Privacy request tenant mismatch.");
        }

        if (entity.Status is CustomerPrivacyRequest.StatusCompleted or CustomerPrivacyRequest.StatusCanceled)
        {
            return entity.ToResponse();
        }

        await privacyRequestRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            if (entity.Status == CustomerPrivacyRequest.StatusFailed)
            {
                entity.CompensateToPendingForRetry(currentUser.UserId, request.Reason);
                await privacyRequestRepository.UpdateAsync(entity, cancellationToken);
                await privacyRequestRepository.SaveChangesAsync(cancellationToken);
            }

            if (entity.Status == CustomerPrivacyRequest.StatusPending)
            {
                await executionService.ExecuteAsync(entity, cancellationToken);
            }

            await domainOperationalEventPublisher.PublishAsync(
                nameof(CustomerPrivacyRequest),
                entity.Id,
                entity.DequeueOperationalEvents(),
                cancellationToken);

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
