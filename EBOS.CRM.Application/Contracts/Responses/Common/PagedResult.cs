namespace EBOS.CRM.Application.Contracts.Responses.Common;

public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    int Total
);
