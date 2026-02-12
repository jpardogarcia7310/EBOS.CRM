using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Services.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Lead.Commands.QualifyLead;

public class QualifyLeadCommandHandler(ILeadRepository repository, IAuditService auditService,
    ICurrentUserContext currentUser) : IRequestHandler<QualifyLeadCommand, LeadResponse?>
{
    public async Task<LeadResponse?> Handle(QualifyLeadCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        var oldValues = AuditSerialization.Serialize(entity);

        entity.Status = "Qualified";
        if (!string.IsNullOrWhiteSpace(request.LeadRequest.Notes))
        {
            entity.Notes = request.LeadRequest.Notes;
        }

        await repository.BeginTransactionAsync(cancellationToken);

        try
        {
            await repository.UpdateAsync(entity, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            var auditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Patch,
                Entity: nameof(Domain.Entities.CRM.Lead),
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

        return new LeadResponse(
            entity.Id,
            entity.TenantId,
            entity.Source,
            entity.Status,
            entity.OwnerUserId,
            entity.CompanyName,
            entity.ContactName,
            entity.Email,
            entity.Phone,
            entity.EstimatedValue,
            entity.Notes,
            entity.ConvertedOpportunityId,
            !entity.Erased);
    }
}
