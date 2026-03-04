using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Infrastructure.Services.CRM;
using Moq;

namespace EBOS.CRM.ApiTests.Infrastructure.Services.CRM;

public class CaseRoutingServiceTest
{
    [Fact]
    public async Task RouteAsync_WhenCurrentQueueIsValid_UsesCurrentQueue()
    {
        var repo = new Mock<IQueueRepository>(MockBehavior.Strict);
        repo.Setup(x => x.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Queue { Id = 7, TenantId = 1, IsActive = true, DefaultOwnerUserId = 10 });
        var sut = new CaseRoutingService(repo.Object);

        var result = await sut.RouteAsync(new Case { TenantId = 1, QueueId = 7, OwnerUserId = 55 }, force: false);

        Assert.Equal(7, result.QueueId);
        Assert.Equal(55, result.OwnerUserId);
        Assert.Equal("current-queue", result.Rule);
    }

    [Fact]
    public async Task RouteAsync_WhenForced_SelectsDefaultActiveQueue()
    {
        var repo = new Mock<IQueueRepository>(MockBehavior.Strict);
        repo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Queue>
            {
                new() { Id = 2, TenantId = 1, IsActive = true, DefaultOwnerUserId = null },
                new() { Id = 1, TenantId = 1, IsActive = true, DefaultOwnerUserId = 99 }
            });

        var sut = new CaseRoutingService(repo.Object);
        var result = await sut.RouteAsync(new Case { TenantId = 1, QueueId = 7, OwnerUserId = 55 }, force: true);

        Assert.Equal(1, result.QueueId);
        Assert.Equal(99, result.OwnerUserId);
        Assert.Equal("default-active-queue", result.Rule);
    }

    [Fact]
    public async Task RouteAsync_WhenNoCandidateQueues_Throws()
    {
        var repo = new Mock<IQueueRepository>(MockBehavior.Strict);
        repo.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Queue>());
        var sut = new CaseRoutingService(repo.Object);

        var act = () => sut.RouteAsync(new Case { TenantId = 1 }, force: true);
        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }
}
