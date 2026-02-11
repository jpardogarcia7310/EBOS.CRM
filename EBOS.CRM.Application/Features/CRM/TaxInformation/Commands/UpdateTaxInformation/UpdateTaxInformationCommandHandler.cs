using System;
using System.Threading;
using System.Threading.Tasks;
using EBOS.CRM.Application.Contracts.Requests.Services;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Services.Audit;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.TaxInformation.Commands.UpdateTaxInformation;

public class UpdateTaxInformationCommandHandler(ITaxInformationRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser, IMapper mapper) :
    IRequestHandler<UpdateTaxInformationCommand, TaxInformationResponse?>
{
    public async Task<TaxInformationResponse?> Handle(UpdateTaxInformationCommand request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entityRequest = request.TaxInformationRequest ??
                            throw new ArgumentNullException(nameof(request.TaxInformationRequest));
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return null;

        var oldValues = AuditSerialization.Serialize(entity);
        mapper.Map(entityRequest, entity);

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Update,
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

        return mapper.Map<TaxInformationResponse>(entity);
    }
}




