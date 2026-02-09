using EBOS.CRM.Application.Contracts.Requests.Services;

namespace EBOS.CRM.Application.Services.Commands;

public sealed record CommandExecutionResult<TResponse>(
    TResponse Response,
    Func<AuditInsertRequest?>? BuildAuditRequest);
