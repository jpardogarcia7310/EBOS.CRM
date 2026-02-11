using EBOS.CRM.Application.Contracts.Requests.CRM.Service.Sla;
using EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.CheckSlaBatch;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using Moq;
using CaseEntity = EBOS.CRM.Domain.Entities.CRM.Case;
using SlaEntity = EBOS.CRM.Domain.Entities.CRM.Sla;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Sla.Queries.CheckSlaBatch;

public class CheckSlaBatchQueryHandlerTest
{
    [Fact]
    public async Task Handle_WhenCasesExist_ReturnsPagedSlaChecks()
    {
        var now = new DateTime(2026, 2, 11, 12, 0, 0, DateTimeKind.Utc);
        var cases = new List<CaseEntity>
        {
            new()
            {
                Id = 1,
                TenantId = 1,
                Status = CaseEntity.StatusOpen,
                Priority = CaseEntity.PriorityLow,
                SlaId = 10,
                DueAt = now.AddMinutes(-5)
            },
            new()
            {
                Id = 2,
                TenantId = 1,
                Status = CaseEntity.StatusClosed,
                Priority = CaseEntity.PriorityLow,
                SlaId = 10,
                DueAt = now.AddMinutes(10)
            },
            new()
            {
                Id = 3,
                TenantId = 2,
                Status = CaseEntity.StatusOpen,
                Priority = CaseEntity.PriorityLow,
                SlaId = 11,
                DueAt = now.AddMinutes(20)
            }
        };

        var slas = new List<SlaEntity>
        {
            new()
            {
                Id = 10,
                TenantId = 1,
                Name = "Standard",
                TargetMinutes = 60,
                WarningMinutes = 30,
                IsActive = true
            },
            new()
            {
                Id = 11,
                TenantId = 2,
                Name = "Other",
                TargetMinutes = 60,
                WarningMinutes = 30,
                IsActive = true
            }
        };

        var caseRepositoryMock = new Mock<ICaseRepository>();
        var slaRepositoryMock = new Mock<ISlaRepository>();

        caseRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<CaseEntity>)cases);

        slaRepositoryMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyCollection<SlaEntity>)slas);

        var handler = new CheckSlaBatchQueryHandler(caseRepositoryMock.Object, slaRepositoryMock.Object);
        var request = new CheckSlaBatchRequest(TenantId: 1, Now: now, PageNumber: 1, PageSize: 10);

        var result = await handler.Handle(new CheckSlaBatchQuery(request), CancellationToken.None);

        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
        var item = result.Items.First();
        Assert.Equal(1, item.CaseId);
        Assert.True(item.IsBreached);
        Assert.True(item.IsActive);
    }

}
