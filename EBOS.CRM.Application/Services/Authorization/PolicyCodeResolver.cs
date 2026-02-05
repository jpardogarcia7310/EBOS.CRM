using System.Text;

namespace EBOS.CRM.Application.Services.Authorization;

public static class PolicyCodeResolver
{
    public static string Resolve(Type requestType)
    {
        var action = ResolveAction(requestType.Name);
        var (module, resource) = ResolveModuleAndResource(requestType);

        if (string.IsNullOrWhiteSpace(resource))
        {
            return string.Empty;
        }

        var moduleSegment = string.IsNullOrWhiteSpace(module) ? "core" : module;
        return $"{moduleSegment}.{resource}.{action}";
    }

    private static string ResolveAction(string typeName)
    {
        var returnValue = "read";
        
        if (typeName.StartsWith("Add", StringComparison.Ordinal))
        {
            returnValue = "create";
        }

        if (typeName.StartsWith("Update", StringComparison.Ordinal) ||
            typeName.StartsWith("Patch", StringComparison.Ordinal))
        {
            returnValue = "update";
        }

        if (typeName.StartsWith("Delete", StringComparison.Ordinal))
        {
            returnValue = "delete";
        }

        if (typeName.StartsWith("GetAll", StringComparison.Ordinal) ||
            typeName.StartsWith("Get", StringComparison.Ordinal))
        {
            returnValue = "read";
        }

        return returnValue;
    }

    private static (string module, string resource) ResolveModuleAndResource(Type requestType)
    {
        var ns = requestType.Namespace ?? string.Empty;
        if (string.IsNullOrWhiteSpace(ns))
        {
            return (string.Empty, string.Empty);
        }

        var segments = ns.Split('.', StringSplitOptions.RemoveEmptyEntries);
        var featureIndex = Array.FindIndex(segments, s => s == "Features");
        if (featureIndex < 0 || featureIndex + 1 >= segments.Length)
        {
            return (string.Empty, string.Empty);
        }

        var moduleSegment = segments[featureIndex + 1];
        var resourceSegment = moduleSegment;

        var crmModuleSegments = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "CRM",
            "Countries",
            "Statuses",
            "IdentificationType",
            "AddressesType",
            "AddressType"
        };

        var module = crmModuleSegments.Contains(moduleSegment)
            ? "crm"
            : ToKebabCase(moduleSegment);

        if (string.Equals(moduleSegment, "CRM", StringComparison.OrdinalIgnoreCase) &&
            featureIndex + 2 < segments.Length)
        {
            resourceSegment = segments[featureIndex + 2];
        }

        var resource = ToKebabCase(Singularize(resourceSegment));

        return (module, resource);
    }

    private static string Singularize(string value)
    {
        if (value.EndsWith("ies", StringComparison.OrdinalIgnoreCase) && value.Length > 3)
        {
            return value[..^3] + "y";
        }

        if (value.EndsWith("ses", StringComparison.OrdinalIgnoreCase) && value.Length > 3)
        {
            return value[..^2];
        }

        if (value.EndsWith("ss", StringComparison.OrdinalIgnoreCase))
        {
            return value;
        }

        if (value.EndsWith("s", StringComparison.OrdinalIgnoreCase) && value.Length > 1)
        {
            return value[..^1];
        }

        return value;
    }

    private static string ToKebabCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        if (value.All(char.IsUpper))
        {
            return value.ToLowerInvariant();
        }

        var sb = new StringBuilder(value.Length + 8);
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (char.IsUpper(c))
            {
                var prevIsLower = i > 0 && char.IsLower(value[i - 1]);
                var nextIsLower = i + 1 < value.Length && char.IsLower(value[i + 1]);

                if (i > 0 && (prevIsLower || nextIsLower))
                {
                    sb.Append('-');
                }

                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(char.ToLowerInvariant(c));
            }
        }

        return sb.ToString();
    }
}
