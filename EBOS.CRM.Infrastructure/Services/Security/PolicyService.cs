using EBOS.CRM.Domain.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace EBOS.CRM.Infrastructure.Services.Security;

public sealed class PolicyService(CrmDbContext context, IConfiguration configuration, IMemoryCache cache)
    : IPolicyService
{
    private const string AuthEnabledKey = "Authentication:Enabled";
    private readonly CrmDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));

    public async Task EnsureAuthorizedAsync(long userId, string policyCode,
        CancellationToken cancellationToken = default)
    {
        if (!IsAuthorizationEnabled())
        {
            return;
        }

        if (userId <= 0)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var normalizedPolicy = Normalize(policyCode);
        if (string.IsNullOrWhiteSpace(normalizedPolicy))
        {
            return;
        }

        var permissions = await GetUserPermissionsAsync(userId, cancellationToken).ConfigureAwait(false);
        if (!permissions.Contains(normalizedPolicy))
        {
            throw new UnauthorizedAccessException($"Policy '{policyCode}' denied.");
        }
    }

    private bool IsAuthorizationEnabled()
    {
        return _configuration.GetValue(AuthEnabledKey, true);
    }

    private async Task<HashSet<string>> GetUserPermissionsAsync(long userId, CancellationToken cancellationToken)
    {
        var cacheKey = $"iam:permissions:{userId}";
        if (_cache.TryGetValue(cacheKey, out HashSet<string>? cached) && cached is not null)
        {
            return cached;
        }

        var rolePermissionCodes = _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.RolePermissions.Select(rp => rp.Permission.Code));

        var userPolicyPermissionCodes = _context.UserPolicies
            .Where(up => up.UserId == userId)
            .SelectMany(up => up.Policy.PolicyPermissions.Select(pp => pp.Permission.Code));

        var rolePolicyPermissionCodes = _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role.PolicyRoles
                .SelectMany(pr => pr.Policy.PolicyPermissions.Select(pp => pp.Permission.Code)));

        var permissions = await rolePermissionCodes
            .Concat(userPolicyPermissionCodes)
            .Concat(rolePolicyPermissionCodes)
            .Select(code => code.ToLowerInvariant())
            .Distinct()
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        var set = permissions.ToHashSet(StringComparer.OrdinalIgnoreCase);
        _cache.Set(cacheKey, set, TimeSpan.FromMinutes(5));
        return set;
    }

    private static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.Trim();
        if (normalized.StartsWith("policy.", StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized["policy.".Length..];
        }

        return normalized.ToLowerInvariant();
    }
}
