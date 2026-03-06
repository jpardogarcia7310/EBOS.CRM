namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface ILeadConversionValidationService
{
    Task EnsureDependenciesAvailableAsync(
        long tenantId,
        long customerId,
        long stageId,
        CancellationToken cancellationToken = default);
}
