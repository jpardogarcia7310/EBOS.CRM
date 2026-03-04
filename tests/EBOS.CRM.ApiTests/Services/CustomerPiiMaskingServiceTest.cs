using System.Security.Claims;
using EBOS.CRM.Api.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using Microsoft.AspNetCore.Http;

namespace EBOS.CRM.ApiTests.Services;

public class CustomerPiiMaskingServiceTest
{
    [Fact]
    public void CanReadPii_WithPermissionClaim_ReturnsTrue()
    {
        var accessor = BuildAccessor(new[] { new Claim("permissions", "crm.customer.pii.read") });
        var sut = new CustomerPiiMaskingService(accessor);

        Assert.True(sut.CanReadPii());
    }

    [Fact]
    public void Mask_Customer_WhenMaskingEnabledAndNoPermission_MasksEmailAndPhone()
    {
        var accessor = BuildAccessor(Array.Empty<Claim>());
        var sut = new CustomerPiiMaskingService(accessor);
        var response = new CustomerResponse(1, 1, "C-001", "a@b.com", "123456789", 1, true);

        var masked = sut.Mask(response, applyMasking: true);

        Assert.NotEqual(response.Email, masked.Email);
        Assert.NotEqual(response.Phone, masked.Phone);
    }

    private static IHttpContextAccessor BuildAccessor(IEnumerable<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims, authenticationType: "test");
        var principal = new ClaimsPrincipal(identity);
        var context = new DefaultHttpContext { User = principal };
        return new HttpContextAccessor { HttpContext = context };
    }
}
