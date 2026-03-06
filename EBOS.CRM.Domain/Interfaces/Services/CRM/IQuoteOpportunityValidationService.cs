namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface IQuoteOpportunityValidationService
{
    Task EnsureOpportunityAvailableAsync(long tenantId, long opportunityId, CancellationToken cancellationToken = default);
}
