using EBOS.CRM.Api.Helpers;

namespace EBOS.CRM.ApiTests.Helpers;

public class ProblemDetailsDefaultsTest
{
    [Fact]
    public void Constants_AreStable()
    {
        Assert.Equal("Resource not found", ProblemDetailsDefaults.NotFoundTitle);
        Assert.Equal("Invalid pageSize", ProblemDetailsDefaults.InvalidPageSizeTitle);
    }
}
