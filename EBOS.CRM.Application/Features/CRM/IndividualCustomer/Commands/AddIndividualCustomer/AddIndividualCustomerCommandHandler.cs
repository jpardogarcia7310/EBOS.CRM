using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.IndividualCustomer.Commands.AddIndividualCustomer;

public class AddIndividualCustomerCommandHandler(IIndividualCustomerRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser, IMapper mapper, IIndividualCustomerReferenceValidationService referenceValidationService) :
    IRequestHandler<AddIndividualCustomerCommand, IndividualCustomerResponse>
{
    public async Task<IndividualCustomerResponse> Handle(AddIndividualCustomerCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.IndividualCustomerRequest ??
                            throw new ArgumentNullException(nameof(request.IndividualCustomerRequest));
        await referenceValidationService.EnsureReferencesAvailableAsync(
            entityRequest.StatusId,
            entityRequest.IdentificationTypeId,
            entityRequest.CountryId,
            cancellationToken);
        var entity = mapper.Map<global::EBOS.CRM.Domain.Entities.CRM.IndividualCustomer>(entityRequest);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.AddAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Add,
                Entity: nameof(Domain.Entities.CRM.IndividualCustomer),
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

        return mapper.Map<IndividualCustomerResponse>(entity);
    }
}





