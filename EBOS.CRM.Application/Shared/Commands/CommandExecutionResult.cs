using EBOS.CRM.Contracts.Requests.Services;

namespace EBOS.CRM.Application.Shared.Commands;

public sealed record CommandExecutionResult<TResponse>(
    TResponse Response,
    Func<AuditInsertRequest?>? BuildAuditRequest);