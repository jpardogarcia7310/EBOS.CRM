using System.Collections;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.CompilerServices;
using EBOS.CRM.Application.Options;
using EBOS.CRM.Domain.Interfaces.Services;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.Application.Behavior;

public class TenantIsolationBehavior<TRequest, TResponse>(ITenantContext tenantContext,
        IOptions<TenantIsolationOptions> options)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly int _maxTraversalDepth =
        Math.Clamp(options.Value.TraversalDepth, options.Value.MinTraversalDepth, options.Value.MaxTraversalDepth);

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (tenantContext.TenantId <= 0 || !TryGetTenantIds(request, out var tenantIds))
        {
            return await next(cancellationToken);
        }

        if (tenantIds.Any(id => id <= 0))
        {
            throw BuildValidationException("TenantId is required.");
        }

        if (tenantIds.Any(id => id != tenantContext.TenantId))
        {
            throw BuildValidationException("TenantId mismatch.");
        }

        return await next(cancellationToken);
    }

    private bool TryGetTenantIds(object request, out IReadOnlyCollection<long> tenantIds)
    {
        var collected = new List<long>();
        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);
        var stack = new Stack<(object Value, int Depth)>();
        stack.Push((request, 0));

        while (stack.Count > 0)
        {
            var (current, depth) = stack.Pop();
            if (current == null || depth > _maxTraversalDepth)
            {
                continue;
            }

            if (!visited.Add(current))
            {
                continue;
            }

            if (TryGetTenantIdFromObject(current, out var tenantId))
            {
                collected.Add(tenantId);
            }

            foreach (var value in GetChildValues(current))
            {
                if (value == null)
                {
                    continue;
                }

                stack.Push((value, depth + 1));
            }
        }

        tenantIds = collected;
        return tenantIds.Count > 0;
    }

    private static bool TryGetTenantIdFromObject(object target, out long tenantId)
    {
        var property = target.GetType().GetProperty("TenantId", BindingFlags.Instance | BindingFlags.Public);
        if (property == null || !property.CanRead)
        {
            tenantId = 0;
            return false;
        }

        var value = property.GetValue(target);
        if (value is long longValue)
        {
            tenantId = longValue;
            return true;
        }

        tenantId = 0;
        return false;
    }

    private static ValidationException BuildValidationException(string message)
    {
        var failure = new ValidationFailure("tenantId", message)
        {
            ErrorCode = ComputeStableCode("tenantId", message)
        };
        return new ValidationException([failure]);
    }

    private static IEnumerable<object?> GetChildValues(object target)
    {
        switch (target)
        {
            case string:
                yield break;
            case IEnumerable enumerable:
                {
                    foreach (var item in enumerable)
                    {
                        yield return item;
                    }

                    yield break;
                }
        }

        var properties = target.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => p.CanRead && p.GetIndexParameters().Length == 0);

        foreach (var property in properties)
        {
            yield return property.GetValue(target);
        }
    }

    private static string ComputeStableCode(string key, string message)
    {
        var payload = $"{key}|{message}";
        var bytes = Encoding.UTF8.GetBytes(payload);
        var hash = SHA256.HashData(bytes);
        var hex = Convert.ToHexString(hash);
        return $"VAL_{hex.Substring(0, 12)}";
    }

    private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public static readonly ReferenceEqualityComparer Instance = new();

        public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);

        public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }
}
