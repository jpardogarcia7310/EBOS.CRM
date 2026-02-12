namespace EBOS.CRM.Contracts.Responses.CRM;

public record SlaCheckResponse(
    long CaseId,
    long SlaId,
    DateTime? DueAt,
    bool IsBreached,
    bool IsActive
);
