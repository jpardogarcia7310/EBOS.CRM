namespace EBOS.CRM.Api.Options;

public sealed class PaginationOptions
{
    public int DefaultPageSize { get; init; } = 50;
    public int MaxPageSize { get; init; } = 200;
}
