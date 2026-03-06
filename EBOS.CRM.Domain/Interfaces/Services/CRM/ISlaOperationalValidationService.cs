namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface ISlaOperationalValidationService
{
    Task<int> CountOpenCasesForSlaDeactivationAsync(long slaId, bool targetIsActive, CancellationToken cancellationToken = default);
}
