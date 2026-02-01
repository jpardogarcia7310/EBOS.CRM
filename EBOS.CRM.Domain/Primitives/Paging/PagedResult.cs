namespace EBOS.CRM.Domain.Primitives.Paging;

public record PagedResult<T>(IReadOnlyCollection<T> Items, int PageNumber, int PageSize, int TotalCount, int TotalPages,
    string? SortBy, string? SortDirection, string? Filter)
{
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}
