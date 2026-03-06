using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Shared.Audit;
using EBOS.CRM.Application.Shared.Observability;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Lead.Commands.ConvertLead;

public class ConvertLeadCommandHandler(ILeadRepository leadRepository, IOpportunityRepository opportunityRepository,
    IAuditService auditService, ICurrentUserContext currentUser, IMapper mapper, IDomainOperationalEventPublisher? domainOperationalEventPublisher = null)
    : IRequestHandler<ConvertLeadCommand, OpportunityResponse?>
{
    public async Task<OpportunityResponse?> Handle(ConvertLeadCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var lead = await leadRepository.GetByIdAsync(request.Id, cancellationToken);
        if (lead is null)
        {
            return null;
        }
        if (lead.TenantId != request.LeadRequest.TenantId)
        {
            throw new DomainConflictException("Lead tenant mismatch.", "DOMAIN_CONFLICT_LEAD_TENANT_MISMATCH");
        }

        if (lead.ConvertedOpportunityId.HasValue)
        {
            var existing = await opportunityRepository.GetByIdAsync(lead.ConvertedOpportunityId.Value,
                cancellationToken);
            return existing is null ? null : mapper.Map<OpportunityResponse>(existing);
        }

        var oldValues = AuditSerialization.Serialize(lead);

        var opportunity = new Domain.Entities.CRM.Opportunity
        {
            TenantId = request.LeadRequest.TenantId
        };
        opportunity.ApplyUpdate(
            request.LeadRequest.OpportunityName,
            request.LeadRequest.StageId,
            lead.OwnerUserId,
            request.LeadRequest.CustomerId,
            request.LeadRequest.ExpectedCloseDate,
            request.LeadRequest.Amount,
            request.LeadRequest.Probability,
            lead.Source,
            lead.Id,
            closeReason: null);

        await leadRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            await opportunityRepository.AddAsync(opportunity, cancellationToken);
            await opportunityRepository.SaveChangesAsync(cancellationToken);

            lead.MarkConverted(opportunity.Id, request.LeadRequest.Notes);
            await leadRepository.UpdateAsync(lead, cancellationToken);
            await leadRepository.SaveChangesAsync(cancellationToken);

            var leadAuditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Patch,
                Entity: nameof(Domain.Entities.CRM.Lead),
                RegisterId: lead.Id,
                OldValues: oldValues,
                NewValues: AuditSerialization.Serialize(lead),
                CorrelationId: currentUser.CorrelationId);

            var opportunityAuditRequest = new AuditInsertRequest(
                UserId: currentUser.UserId,
                TimeStamp: DateTimeOffset.UtcNow,
                Action: AuditActions.Add,
                Entity: nameof(Domain.Entities.CRM.Opportunity),
                RegisterId: opportunity.Id,
                OldValues: null,
                NewValues: AuditSerialization.Serialize(opportunity),
                CorrelationId: currentUser.CorrelationId);

            await auditService.InsertAuditAsync(opportunityAuditRequest, cancellationToken);
            await auditService.InsertAuditAsync(leadAuditRequest, cancellationToken);
            if (domainOperationalEventPublisher is not null)
            {
                await domainOperationalEventPublisher.PublishAsync(
                    nameof(Domain.Entities.CRM.Opportunity),
                    opportunity.Id,
                    opportunity.DequeueOperationalEvents(),
                    cancellationToken);
                await domainOperationalEventPublisher.PublishAsync(
                    nameof(Domain.Entities.CRM.Lead),
                    lead.Id,
                    lead.DequeueOperationalEvents(),
                    cancellationToken);
            }

            await leadRepository.CommitAsync(cancellationToken);
        }
        catch
        {
            await leadRepository.RollbackAsync(cancellationToken);
            throw;
        }

        return mapper.Map<OpportunityResponse>(opportunity);
    }
}
