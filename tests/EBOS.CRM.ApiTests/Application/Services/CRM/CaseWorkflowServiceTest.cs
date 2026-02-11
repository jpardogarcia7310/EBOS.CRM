using EBOS.CRM.Application.Options;
using EBOS.CRM.Application.Services.CRM;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Services.CRM;

public class CaseWorkflowServiceTest
{
    [Fact]
    public async Task EnsureCanTransitionAsync_WhenClosingWithOpenActivitiesAndNotAllowed_Throws()
    {
        var activityRepository = new Mock<ICaseActivityRepository>();
        activityRepository.Setup(r => r.HasOpenByCaseIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var options = Microsoft.Extensions.Options.Options.Create(new CaseWorkflowOptions
        {
            AllowCloseWithOpenActivities = false
        });

        var service = new CaseWorkflowService(activityRepository.Object, options);
        var entity = new Case { Id = 10, Status = Case.StatusOpen, Priority = Case.PriorityLow };

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.EnsureCanTransitionAsync(entity, Case.StatusClosed, CancellationToken.None));
    }

    [Fact]
    public async Task EnsureCanTransitionAsync_WhenClosingWithOpenActivitiesAndAllowed_DoesNotThrow()
    {
        var activityRepository = new Mock<ICaseActivityRepository>();
        activityRepository.Setup(r => r.HasOpenByCaseIdAsync(11, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var options = Microsoft.Extensions.Options.Options.Create(new CaseWorkflowOptions
        {
            AllowCloseWithOpenActivities = true
        });

        var service = new CaseWorkflowService(activityRepository.Object, options);
        var entity = new Case { Id = 11, Status = Case.StatusOpen, Priority = Case.PriorityLow };

        await service.EnsureCanTransitionAsync(entity, Case.StatusClosed, CancellationToken.None);
    }
}
