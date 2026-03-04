namespace EBOS.CRM.Contracts.Responses.CRM;

public record QueueResponse(
    long Id,
    long TenantId,
    string Name,
    string Code,
    bool IsActive,
    long? DefaultOwnerUserId,
    bool Active
);
