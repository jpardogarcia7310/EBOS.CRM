using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Application.Services.Audit;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using MediatR;

namespace EBOS.CRM.Application.Features.CRM.Lead.Commands.ConvertLead;

public class ConvertLeadCommandHandler(ILeadRepository leadRepository, IOpportunityRepository opportunityRepository,
    IAuditService auditService, ICurrentUserContext currentUser, IMapper mapper)
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

        if (lead.ConvertedOpportunityId.HasValue)
        {
            var existing = await opportunityRepository.GetByIdAsync(lead.ConvertedOpportunityId.Value,
                cancellationToken);
            return existing is null ? null : mapper.Map<OpportunityResponse>(existing);
        }

        var oldValues = AuditSerialization.Serialize(lead);

        var opportunity = new Domain.Entities.CRM.Opportunity
        {
            TenantId = request.LeadRequest.TenantId,
            Name = request.LeadRequest.OpportunityName,
            StageId = request.LeadRequest.StageId,
            OwnerUserId = lead.OwnerUserId,
            CustomerId = request.LeadRequest.CustomerId,
            ExpectedCloseDate = request.LeadRequest.ExpectedCloseDate,
            Amount = request.LeadRequest.Amount,
            Probability = request.LeadRequest.Probability,
            Source = lead.Source,
            SourceLeadId = lead.Id
        };

        lead.Status = "Converted";
        if (!string.IsNullOrWhiteSpace(request.LeadRequest.Notes))
        {
            lead.Notes = request.LeadRequest.Notes;
        }

        await leadRepository.BeginTransactionAsync(cancellationToken);

        try
        {
            await opportunityRepository.AddAsync(opportunity, cancellationToken);
            await opportunityRepository.SaveChangesAsync(cancellationToken);

            lead.ConvertedOpportunityId = opportunity.Id;
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
