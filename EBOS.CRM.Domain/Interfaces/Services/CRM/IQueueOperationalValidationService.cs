namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface IQueueOperationalValidationService
{
    Task<int> CountOpenCasesForQueueDeactivationAsync(long queueId, bool targetIsActive, CancellationToken cancellationToken = default);
}
