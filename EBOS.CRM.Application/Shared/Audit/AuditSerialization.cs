using System.Text.Json;
using System.Text.Json.Serialization;

namespace EBOS.CRM.Application.Shared.Audit;

public static class AuditSerialization
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = false
    };

    public static string? Serialize(object? value)
        => value == null ? null : JsonSerializer.Serialize(value, Options);
}

