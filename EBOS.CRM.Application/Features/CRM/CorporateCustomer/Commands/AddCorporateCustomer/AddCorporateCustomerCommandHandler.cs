using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.CorporateCustomer.Commands.AddCorporateCustomer;

public class AddCorporateCustomerCommandHandler(ICorporateCustomerRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser, IMapper mapper, ICustomerReferenceValidationService referenceValidationService) :
    IRequestHandler<AddCorporateCustomerCommand, CorporateCustomerResponse>
{
    public async Task<CorporateCustomerResponse> Handle(AddCorporateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.CorporateCustomerRequest ??
                            throw new ArgumentNullException(nameof(request.CorporateCustomerRequest));
        await referenceValidationService.EnsureStatusAndCountryAvailableAsync(
            entityRequest.StatusId,
            entityRequest.CountryId,
            cancellationToken);
        var entity = mapper.Map<global::EBOS.CRM.Domain.Entities.CRM.CorporateCustomer>(entityRequest);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.AddAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Add,
                Entity: nameof(Domain.Entities.CRM.CorporateCustomer),
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

        return mapper.Map<CorporateCustomerResponse>(entity);
    }
}




