using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Entities.EBOS;

namespace EBOS.CRM.Domain.Interfaces.Services.EBOS;

public interface IAuditOutboxValidationService
{
    void EnsureEnqueueRequestIsValid(string operation, AuditInsertRequest request);
    AuditInsertRequest EnsureDispatchPayloadIsValid(AuditOutboxMessage message);
}
