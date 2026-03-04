using EBOS.CRM.Application.Behavior;
using EBOS.CRM.Application.Options;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.EBOS;
using FluentValidation;
using MediatR;
using OptionsProvider = Microsoft.Extensions.Options.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EBOS.CRM.ApiTests.Application.Behavior;

public class TenantIsolationBehaviorTests
{
    [Fact]
    public async Task Handle_Allows_WhenTenantMatches()
    {
        var behavior = new TenantIsolationBehavior<TestRequest, Unit>(
            new TestTenantContext(5),
            OptionsProvider.Create(new TenantIsolationOptions()));

        var result = await behavior.Handle(new TestRequest(5), _ => Task.FromResult(Unit.Value),
            CancellationToken.None);

        Assert.Equal(Unit.Value, result);
    }

    [Fact]
    public async Task Handle_Throws_WhenTenantMissing()
    {
        var behavior = new TenantIsolationBehavior<TestRequest, Unit>(
            new TestTenantContext(5),
            OptionsProvider.Create(new TenantIsolationOptions()));

        var act = () => behavior.Handle(new TestRequest(0), _ => Task.FromResult(Unit.Value),
            CancellationToken.None);

        var ex = await Assert.ThrowsAsync<ValidationException>(act);
        Assert.Equal("VAL_6FE10CA408C5", ex.Errors.Single().ErrorCode);
    }

    [Fact]
    public async Task Handle_Throws_WhenTenantMismatch()
    {
        var behavior = new TenantIsolationBehavior<TestRequest, Unit>(
            new TestTenantContext(5),
            OptionsProvider.Create(new TenantIsolationOptions()));

        var act = () => behavior.Handle(new TestRequest(7), _ => Task.FromResult(Unit.Value), CancellationToken.None);

        var ex = await Assert.ThrowsAsync<ValidationException>(act);
        Assert.Equal("VAL_B86E9AF1B187", ex.Errors.Single().ErrorCode);
    }

    [Fact]
    public async Task Handle_Allows_WhenTenantContextMissing()
    {
        var behavior = new TenantIsolationBehavior<TestRequest, Unit>(
            new TestTenantContext(0),
            OptionsProvider.Create(new TenantIsolationOptions()));

        var result = await behavior.Handle(new TestRequest(0), _ => Task.FromResult(Unit.Value),
            CancellationToken.None);

        Assert.Equal(Unit.Value, result);
    }

    [Fact]
    public async Task Handle_UsesNestedRequestTenantId()
    {
        var behavior = new TenantIsolationBehavior<OuterRequest, Unit>(
            new TestTenantContext(3),
            OptionsProvider.Create(new TenantIsolationOptions()));

        var result = await behavior.Handle(new OuterRequest(new InnerRequest(3)), _ => Task.FromResult(Unit.Value),
            CancellationToken.None);

        Assert.Equal(Unit.Value, result);
    }

    [Fact]
    public async Task Handle_UsesDeepNestedCollectionTenantId()
    {
        var behavior = new TenantIsolationBehavior<OuterRequest, Unit>(
            new TestTenantContext(3),
            OptionsProvider.Create(new TenantIsolationOptions()));

        var request = new OuterRequest(new InnerRequest(3))
        {
            Nested = new[]
            {
                new DeepRequest(new InnerRequest(3)),
                new DeepRequest(new InnerRequest(3))
            }
        };

        var result = await behavior.Handle(request, _ => Task.FromResult(Unit.Value), CancellationToken.None);

        Assert.Equal(Unit.Value, result);
    }

    [Fact]
    public async Task Handle_EnforcesTraversalDepth_FromAppSettings()
    {
        var options = LoadTenantIsolationOptions();
        var behavior = new TenantIsolationBehavior<WrapperLevel1, Unit>(new TestTenantContext(5), options);

        var request = new WrapperLevel1(
            new WrapperLevel2(new InnerRequest(7)));
        var act = () => behavior.Handle(request, _ => Task.FromResult(Unit.Value), CancellationToken.None);

        if (options.Value.TraversalDepth >= 2)
        {
            await Assert.ThrowsAsync<ValidationException>(act);
            return;
        }

        var result = await act();
        Assert.Equal(Unit.Value, result);
    }

    private sealed record TestRequest(long TenantId);

    private sealed record OuterRequest(InnerRequest Request)
    {
        public IEnumerable<DeepRequest>? Nested { get; init; }
    }

    private sealed record InnerRequest(long TenantId);

    private sealed record DeepRequest(InnerRequest Payload)
    {
        public DepthRequest? Next { get; init; }
    }

    private sealed record DepthRequest(InnerRequest Payload);

    private sealed record WrapperLevel1(WrapperLevel2 Child);

    private sealed record WrapperLevel2(InnerRequest Payload);

    private sealed class TestTenantContext(long tenantId) : ITenantContext
    {
        public long TenantId => tenantId;
    }

    private static IOptions<TenantIsolationOptions> LoadTenantIsolationOptions()
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "EBOS.CRM.Api");
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var options = new TenantIsolationOptions();
        configuration.GetSection(TenantIsolationOptions.SectionName).Bind(options);
        return OptionsProvider.Create(options);
    }
}
