using System.Linq.Expressions;
using EBOS.CRM.Domain.Primitives.Paging;
using Microsoft.EntityFrameworkCore;

namespace EBOS.CRM.Infrastructure.Repositories;

internal static class PagedQueryExtensions
{
    public static async Task<PagedResult<T>> ApplyPagedQueryAsync<T>(
        this IQueryable<T> query,
        PagedQuery queryOptions,
        CancellationToken cancellationToken)
    {
        var normalized = queryOptions.Normalize();

        var filtered = ApplyFilter(query, normalized.Filter);
        var sorted = ApplySort(filtered, normalized.SortBy, normalized.SortDirection ?? "asc");

        var totalCount = await sorted.CountAsync(cancellationToken);
        var pageNumber = normalized.PageNumber;
        var pageSize = normalized.PageSize;
        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await sorted
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>(
            items,
            pageNumber,
            pageSize,
            totalCount,
            totalPages,
            normalized.SortBy,
            normalized.SortDirection,
            normalized.Filter);
    }

    private static IQueryable<T> ApplyFilter<T>(IQueryable<T> query, string? filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            return query;
        }

        var segments = filter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var segment in segments)
        {
            if (string.IsNullOrWhiteSpace(segment))
            {
                continue;
            }

            var index = segment.IndexOf(':');
            if (index > 0)
            {
                var propertyName = segment[..index].Trim();
                var value = segment[(index + 1)..].Trim();
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                query = ApplyPropertyFilter(query, propertyName, value);
                continue;
            }

            query = ApplyGlobalSearch(query, segment.Trim());
        }

        return query;
    }

    private static IQueryable<T> ApplyPropertyFilter<T>(IQueryable<T> query, string propertyName, string value)
    {
        var property = ResolveProperty(typeof(T), propertyName);
        if (property is null)
        {
            return query;
        }

        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyAccess = Expression.Property(parameter, property);
        Expression? predicateBody = null;

        if (property.PropertyType == typeof(string))
        {
            var upperValue = Expression.Constant(value.ToUpperInvariant());
            var notNull = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));
            var toUpper = Expression.Call(propertyAccess, nameof(string.ToUpperInvariant), Type.EmptyTypes);
            var contains = Expression.Call(toUpper, nameof(string.Contains), Type.EmptyTypes, upperValue);
            predicateBody = Expression.AndAlso(notNull, contains);
        }
        else
        {
            var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            object? converted;
            try
            {
                converted = Convert.ChangeType(value, targetType);
            }
            catch
            {
                return query;
            }

            var constant = Expression.Constant(converted, targetType);
            Expression comparison = Expression.Equal(Expression.Convert(propertyAccess, targetType), constant);
            predicateBody = comparison;
        }

        if (predicateBody is null)
        {
            return query;
        }

        var predicate = Expression.Lambda<Func<T, bool>>(predicateBody, parameter);
        return query.Where(predicate);
    }

    private static IQueryable<T> ApplyGlobalSearch<T>(IQueryable<T> query, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return query;
        }

        var stringProperties = typeof(T)
            .GetProperties()
            .Where(p => p.PropertyType == typeof(string))
            .ToArray();
        if (stringProperties.Length == 0)
        {
            return query;
        }

        var parameter = Expression.Parameter(typeof(T), "x");
        var upperValue = Expression.Constant(value.ToUpperInvariant());
        Expression? predicateBody = null;

        foreach (var property in stringProperties)
        {
            var propertyAccess = Expression.Property(parameter, property);
            var notNull = Expression.NotEqual(propertyAccess, Expression.Constant(null, typeof(string)));
            var toUpper = Expression.Call(propertyAccess, nameof(string.ToUpperInvariant), Type.EmptyTypes);
            var contains = Expression.Call(toUpper, nameof(string.Contains), Type.EmptyTypes, upperValue);
            var condition = Expression.AndAlso(notNull, contains);
            predicateBody = predicateBody is null ? condition : Expression.OrElse(predicateBody, condition);
        }

        if (predicateBody is null)
        {
            return query;
        }

        var predicate = Expression.Lambda<Func<T, bool>>(predicateBody, parameter);
        return query.Where(predicate);
    }

    private static IQueryable<T> ApplySort<T>(IQueryable<T> query, string? sortBy, string sortDirection)
    {
        var property = ResolveProperty(typeof(T), sortBy)
                       ?? ResolveProperty(typeof(T), "Id")
                       ?? typeof(T).GetProperties().FirstOrDefault();
        if (property is null)
        {
            return query;
        }

        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyAccess = Expression.Property(parameter, property);
        var lambda = Expression.Lambda(propertyAccess, parameter);
        var methodName = sortDirection == "desc" ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);
        var methodCall = Expression.Call(
            typeof(Queryable),
            methodName,
            [typeof(T), property.PropertyType],
            query.Expression,
            Expression.Quote(lambda));

        return query.Provider.CreateQuery<T>(methodCall);
    }

    private static System.Reflection.PropertyInfo? ResolveProperty(Type type, string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        return type.GetProperties()
            .FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
    }
}
