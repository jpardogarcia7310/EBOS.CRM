namespace EBOS.CRM.Domain.Interfaces.Services.CRM;

public interface ICustomer360Metrics
{
    void RecordMerge(long tenantId, int mergedCount, bool success);
    void RecordDedupeQuery(long tenantId, int candidateCount, int scoreThreshold);
    void RecordConsentEvent(long tenantId, string consentType, bool granted);
    void RecordAuditOutboxEnqueue(string operation);
    void RecordAuditOutboxDispatch(string operation, bool success);
    void RecordConcurrencyConflict(bool exhaustedRetries);
    Customer360MetricsSnapshot GetSnapshot();
}
