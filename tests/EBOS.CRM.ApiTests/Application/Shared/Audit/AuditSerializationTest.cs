using EBOS.CRM.Application.Shared.Audit;

namespace EBOS.CRM.ApiTests.Application.Shared.Audit;

public class AuditSerializationTest
{
    [Fact]
    public void Serialize_Null_ReturnsNull()
    {
        var result = AuditSerialization.Serialize(null);
        Assert.Null(result);
    }

    [Fact]
    public void Serialize_Object_UsesCamelCase()
    {
        var payload = new SamplePayload("Value", 10);
        var result = AuditSerialization.Serialize(payload);

        Assert.NotNull(result);
        Assert.Contains("\"sampleName\":\"Value\"", result);
        Assert.Contains("\"sampleCount\":10", result);
    }

    private sealed record SamplePayload(string SampleName, int SampleCount);
}
