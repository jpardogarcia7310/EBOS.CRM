using EBOS.CRM.Application.Contracts.Requests.Services;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Services.Audit;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.PatchTaxInformation;

public class PatchTaxInformationCommandHandler(ITaxInformationRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser) : IRequestHandler<PatchTaxInformationCommand, TaxInformationResponse?>
{
    public async Task<TaxInformationResponse?> Handle(PatchTaxInformationCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var oldValues = AuditSerialization.Serialize(entity);

        if (request.TaxInformationRequest.TaxName != null)
            entity.TaxName = request.TaxInformationRequest.TaxName;
        if (request.TaxInformationRequest.TaxIdentificationNumber != null)
            entity.TaxIdentificationNumber = request.TaxInformationRequest.TaxIdentificationNumber;
        if (request.TaxInformationRequest.CustomerId.HasValue)
            entity.CustomerId = request.TaxInformationRequest.CustomerId.Value;

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Patch,
                Entity: nameof(Domain.Entities.CRM.TaxInformation),
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

        return new TaxInformationResponse(
            entity.Id,
            entity.TaxName,
            entity.TaxIdentificationNumber,
            entity.CustomerId,
            !entity.Erased);
    }
}




