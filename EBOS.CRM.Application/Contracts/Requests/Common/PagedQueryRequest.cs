using EBOS.CRM.Domain.Primitives.Paging;

namespace EBOS.CRM.Application.Contracts.Requests.Common;

public record PagedQueryRequest
{
    public int PageNumber { get; init; } = PagedQuery.DefaultPageNumber;
    public int PageSize { get; init; } = PagedQuery.DefaultPageSize;
    public string? SortBy { get; init; }
    public string? SortDirection { get; init; }
    public string? Filter { get; init; }

    public PagedQuery ToPagedQuery()
        => new(PageNumber, PageSize, SortBy, SortDirection, Filter);
}
