using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services.CRM;

namespace EBOS.CRM.Application.Features.CRM.Service.Sla.Services;

public sealed class SlaOperationalValidationService(ICaseRepository caseRepository) : ISlaOperationalValidationService
{
    public async Task<int> CountOpenCasesForSlaDeactivationAsync(long slaId, bool targetIsActive, CancellationToken cancellationToken = default)
    {
        try
        {
            return targetIsActive ? 0 : await caseRepository.CountOpenBySlaIdAsync(slaId, cancellationToken);
        }
        catch (Exception ex) when (
            ex is not DomainException &&
            DomainTransientFailureClassifier.TryClassify(ex, nameof(CountOpenCasesForSlaDeactivationAsync), out _))
        {
            throw new TransientDomainFailureException(
                "Transient failure while validating SLA operational constraints.",
                "DOMAIN_TRANSIENT_SLA_OPERATIONAL_VALIDATION",
                ex);
        }
    }
}
