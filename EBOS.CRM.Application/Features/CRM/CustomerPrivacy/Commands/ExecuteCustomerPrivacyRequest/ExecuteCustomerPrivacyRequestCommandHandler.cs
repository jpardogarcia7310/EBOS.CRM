using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Features.CRM.CustomerPrivacy;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Application.Shared.Observability;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CustomerPrivacy.Commands.ExecuteCustomerPrivacyRequest;

public sealed class ExecuteCustomerPrivacyRequestCommandHandler(
    ICustomerPrivacyRequestRepository privacyRequestRepository,
    CustomerPrivacyExecutionService executionService,
    IDomainOperationalEventPublisher domainOperationalEventPublisher)
    : IRequestHandler<ExecuteCustomerPrivacyRequestCommand, CustomerPrivacyRequestResponse?>
{
    public async Task<CustomerPrivacyRequestResponse?> Handle(ExecuteCustomerPrivacyRequestCommand command,
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
            throw new DomainConflictException("Privacy request tenant mismatch.", "DOMAIN_CONFLICT_PRIVACY_REQUEST_TENANT_MISMATCH");
        }

        if (entity.Status is CustomerPrivacyRequest.StatusCompleted or CustomerPrivacyRequest.StatusCanceled)
        {
            return entity.ToResponse();
        }

        await privacyRequestRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            await executionService.ExecuteAsync(entity, cancellationToken);
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
