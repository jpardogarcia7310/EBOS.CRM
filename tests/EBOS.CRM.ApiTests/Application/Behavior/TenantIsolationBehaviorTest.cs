using EBOS.CRM.Application.Behavior;
using EBOS.CRM.Application.Options;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;
using FluentValidation;

namespace EBOS.CRM.ApiTests.Application.Behavior;

public class TenantIsolationBehaviorTest
{
    [Fact]
    public async Task Handle_Allows_Request_When_TenantContext_Missing()
    {
        var behavior = BuildBehavior(tenantId: 0);
        var request = new TestRequest { TenantId = 2 };
        var called = false;

        var result = await behavior.Handle(request, _ =>
        {
            called = true;
            return Task.FromResult("ok");
        }, CancellationToken.None);

        Assert.True(called);
        Assert.Equal("ok", result);
    }

    [Fact]
    public async Task Handle_Throws_When_TenantId_Missing_In_Request()
    {
        var behavior = BuildBehavior(tenantId: 3);
        var request = new TestRequest { TenantId = 0 };

        var ex = await Assert.ThrowsAsync<ValidationException>(
            () => behavior.Handle(request, _ => Task.FromResult("ok"), CancellationToken.None));

        var failure = ex.Errors.Single();
        Assert.Equal("TenantId is required.", failure.ErrorMessage);
        Assert.StartsWith("VAL_", failure.ErrorCode);
    }

    [Fact]
    public async Task Handle_Throws_When_TenantId_Mismatch()
    {
        var behavior = BuildBehavior(tenantId: 3);
        var request = new TestRequest { TenantId = 9 };

        var ex = await Assert.ThrowsAsync<ValidationException>(
            () => behavior.Handle(request, _ => Task.FromResult("ok"), CancellationToken.None));

        var failure = ex.Errors.Single();
        Assert.Equal("TenantId mismatch.", failure.ErrorMessage);
        Assert.StartsWith("VAL_", failure.ErrorCode);
    }

    [Fact]
    public async Task Handle_Allows_When_All_TenantIds_Match()
    {
        var behavior = BuildBehavior(tenantId: 7);
        var request = new TestRequest
        {
            TenantId = 7,
            Child = new TestRequest { TenantId = 7 }
        };

        var result = await behavior.Handle(request, _ => Task.FromResult("ok"), CancellationToken.None);

        Assert.Equal("ok", result);
    }

    [Fact]
    public async Task Handle_Skips_TenantId_When_Beyond_Max_Depth()
    {
        var options = new TenantIsolationOptions
        {
            MinTraversalDepth = 1,
            MaxTraversalDepth = 1,
            TraversalDepth = 1
        };
        var behavior = BuildBehavior(tenantId: 5, options);
        var request = new TestRequest
        {
            TenantId = 5,
            Child = new TestRequest
            {
                TenantId = 5,
                Child = new TestRequest { TenantId = 9 }
            }
        };

        var result = await behavior.Handle(request, _ => Task.FromResult("ok"), CancellationToken.None);

        Assert.Equal("ok", result);
    }

    private static TenantIsolationBehavior<TestRequest, string> BuildBehavior(long tenantId, TenantIsolationOptions? options = null)
        => new(new TestTenantContext(tenantId),
            Microsoft.Extensions.Options.Options.Create(options ?? new TenantIsolationOptions
            {
                MinTraversalDepth = 1,
                MaxTraversalDepth = 50,
                TraversalDepth = 10
            }));

    private sealed class TestTenantContext(long tenantId) : ITenantContext
    {
        public long TenantId => tenantId;
    }

    private sealed class TestRequest
    {
        public long TenantId { get; init; }
        public TestRequest? Child { get; init; }
    }
}
