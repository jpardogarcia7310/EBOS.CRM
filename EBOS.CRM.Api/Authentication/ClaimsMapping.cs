using System.Security.Claims;
using System.Text.Json;

namespace EBOS.CRM.Api.Authentication;

public static class ClaimsMapping
{
    public static void MapClaimValues(ClaimsIdentity identity, string sourceClaimType, string targetClaimType)
    {
        if (string.IsNullOrWhiteSpace(sourceClaimType) || string.IsNullOrWhiteSpace(targetClaimType))
        {
            return;
        }

        var sourceClaims = identity.FindAll(sourceClaimType).ToArray();
        if (sourceClaims.Length == 0)
        {
            return;
        }

        foreach (var claim in sourceClaims)
        {
            foreach (var value in ExtractValues(claim.Value))
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                if (!identity.HasClaim(targetClaimType, value))
                {
                    identity.AddClaim(new Claim(targetClaimType, value));
                }
            }
        }
    }

    private static IEnumerable<string> ExtractValues(string? rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            yield break;
        }

        var trimmed = rawValue.Trim();
        if (trimmed.StartsWith("[", StringComparison.Ordinal))
        {
            if (TryExtractJsonArray(trimmed, out var values))
            {
                foreach (var value in values)
                {
                    yield return value;
                }

                yield break;
            }
        }

        if (trimmed.Contains(',', StringComparison.Ordinal) || trimmed.Contains(' ', StringComparison.Ordinal))
        {
            foreach (var value in trimmed.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                yield return value;
            }

            yield break;
        }

        yield return trimmed;
    }

    private static bool TryExtractJsonArray(string rawValue, out List<string> values)
    {
        values = new List<string>();

        try
        {
            using var document = JsonDocument.Parse(rawValue);
            if (document.RootElement.ValueKind != JsonValueKind.Array)
            {
                return false;
            }

            foreach (var element in document.RootElement.EnumerateArray())
            {
                if (element.ValueKind == JsonValueKind.String)
                {
                    var value = element.GetString();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        values.Add(value);
                    }
                }
            }

            return values.Count > 0;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
