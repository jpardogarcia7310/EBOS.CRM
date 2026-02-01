namespace EBOS.CRM.Application.Contracts.Responses.Common;

public record PagedResponse<T>(
    IReadOnlyCollection<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    string? SortBy,
    string? SortDirection,
    string? Filter)
{
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}
