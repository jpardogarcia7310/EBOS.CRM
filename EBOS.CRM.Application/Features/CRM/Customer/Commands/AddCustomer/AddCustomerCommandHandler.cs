using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Customer.Commands.AddCustomer;

public class AddCustomerCommandHandler(ICustomerRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser, IMapper mapper, ICustomerReferenceValidationService referenceValidationService)
    : IRequestHandler<AddCustomerCommand, CustomerResponse>
{
    public async Task<CustomerResponse> Handle(AddCustomerCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.CustomerRequest ?? throw new ArgumentNullException(nameof(request.CustomerRequest));
        await referenceValidationService.EnsureStatusAndCountryAvailableAsync(
            entityRequest.StatusId,
            entityRequest.CountryId,
            cancellationToken);
        var entity = mapper.Map<global::EBOS.CRM.Domain.Entities.CRM.Customer>(entityRequest);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.AddAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Add,
                Entity: nameof(Domain.Entities.CRM.Customer),
                RegisterId: entity.Id,
                OldValues: null,
                NewValues: AuditSerialization.Serialize(entity),
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(auditRequest, cancellationToken);
            await repository.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await repository.RollbackAsync(cancellationToken);

            if (DomainTransientFailureClassifier.TryClassify(ex, nameof(Handle), out var transient))
            {
                throw transient;
            }

            throw;
        }

        return mapper.Map<CustomerResponse>(entity);
    }
}




