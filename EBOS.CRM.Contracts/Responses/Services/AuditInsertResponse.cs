namespace EBOS.CRM.Contracts.Responses.Services;

public sealed record AuditInsertResponse(
    bool Success,
    long Id
);