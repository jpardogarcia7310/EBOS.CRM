namespace EBOS.CRM.Application.Contracts.Responses.CRM;

public record SlaCheckResponse(
    long CaseId,
    long SlaId,
    DateTime? DueAt,
    bool IsBreached,
    bool IsActive
);
