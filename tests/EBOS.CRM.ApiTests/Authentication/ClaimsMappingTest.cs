using System.Security.Claims;
using EBOS.CRM.Api.Authentication;

namespace EBOS.CRM.ApiTests.Authentication;

public class ClaimsMappingTest
{
    [Fact]
    public void MapClaimValues_JsonArray_MapsDistinctValues()
    {
        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim("roles_src", "[\"admin\",\"ops\",\"admin\"]"));

        ClaimsMapping.MapClaimValues(identity, "roles_src", ClaimTypes.Role);

        var roles = identity.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();
        Assert.Equal(2, roles.Length);
        Assert.Contains("admin", roles);
        Assert.Contains("ops", roles);
    }

    [Fact]
    public void MapClaimValues_CommaAndSpaceSeparated_MapsValues()
    {
        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim("permissions_src", "read, write delete"));

        ClaimsMapping.MapClaimValues(identity, "permissions_src", "permission");

        var values = identity.FindAll("permission").Select(c => c.Value).ToArray();
        Assert.Equal(3, values.Length);
        Assert.Contains("read", values);
        Assert.Contains("write", values);
        Assert.Contains("delete", values);
    }

    [Fact]
    public void MapClaimValues_InvalidInput_DoesNothing()
    {
        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim("roles_src", "admin"));

        ClaimsMapping.MapClaimValues(identity, "", ClaimTypes.Role);
        ClaimsMapping.MapClaimValues(identity, "roles_src", "");
        ClaimsMapping.MapClaimValues(identity, "missing", ClaimTypes.Role);

        Assert.Empty(identity.FindAll(ClaimTypes.Role));
    }
}
