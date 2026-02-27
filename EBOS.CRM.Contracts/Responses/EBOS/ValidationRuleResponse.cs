namespace EBOS.CRM.Contracts.Responses.EBOS;

public record ValidationRuleResponse(
    long Id,
    string Key,
    string Pattern,
    string? Description,
    bool IsActive
);
