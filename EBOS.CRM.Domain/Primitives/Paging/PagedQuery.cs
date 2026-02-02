namespace EBOS.CRM.Domain.Primitives.Paging;

public record PagedQuery(int PageNumber = 1, int PageSize = 50, string? SortBy = null,
    string? SortDirection = null, string? Filter = null)
{
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 50;
    public const int MaxPageSize = 200;

    public PagedQuery Normalize()
    {
        var pageNumber = PageNumber < 1 ? DefaultPageNumber : PageNumber;
        var pageSize = PageSize < 1 ? DefaultPageSize : PageSize;
        if (pageSize > MaxPageSize)
        {
            pageSize = MaxPageSize;
        }

        var sortDirection = string.IsNullOrWhiteSpace(SortDirection)
            ? "asc"
            : SortDirection.Trim().ToLowerInvariant();
        if (sortDirection is not ("asc" or "desc"))
        {
            sortDirection = "asc";
        }

        var sortBy = string.IsNullOrWhiteSpace(SortBy) ? null : SortBy.Trim();
        var filter = string.IsNullOrWhiteSpace(Filter) ? null : Filter.Trim();

        return new PagedQuery(PageNumber: pageNumber, PageSize: pageSize, SortBy: sortBy, SortDirection: sortDirection,
            Filter: filter);
    }
}
