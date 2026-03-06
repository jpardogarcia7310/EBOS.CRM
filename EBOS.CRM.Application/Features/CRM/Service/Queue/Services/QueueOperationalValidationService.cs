using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Application.Features.CRM.Service.Queue.Services;

public sealed class QueueOperationalValidationService(ICaseRepository caseRepository) : IQueueOperationalValidationService
{
    public async Task<int> CountOpenCasesForQueueDeactivationAsync(long queueId, bool targetIsActive, CancellationToken cancellationToken = default)
    {
        try
        {
            return targetIsActive ? 0 : await caseRepository.CountOpenByQueueIdAsync(queueId, cancellationToken);
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, nameof(CountOpenCasesForQueueDeactivationAsync), out _))
        {
            throw new TransientDomainFailureException(
                "Transient failure while validating queue operational constraints.",
                "DOMAIN_TRANSIENT_QUEUE_OPERATIONAL_VALIDATION",
                ex);
        }
    }
}
