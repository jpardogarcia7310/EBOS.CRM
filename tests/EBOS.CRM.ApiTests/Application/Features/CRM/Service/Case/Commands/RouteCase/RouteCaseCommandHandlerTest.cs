using EBOS.CRM.Application.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Application.Contracts.Responses.CRM;
using EBOS.CRM.Application.Contracts.Responses.Services;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.RouteCase;
using EBOS.CRM.Application.Services.Interfaces;
using EBOS.CRM.Domain.Entities.CRM;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using MapsterMapper;
using Moq;
using CaseEntity = EBOS.CRM.Domain.Entities.CRM.Case;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Case.Commands.RouteCase;

public class RouteCaseCommandHandlerTest
{
    private readonly Mock<ICaseRepository> _repositoryMock;
    private readonly Mock<ICaseRoutingService> _routingServiceMock;
    private readonly Mock<IAuditService> _auditServiceMock;
    private readonly Mock<ICurrentUserContext> _currentUserMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly RouteCaseCommandHandler _handler;

    public RouteCaseCommandHandlerTest()
    {
        _repositoryMock = new Mock<ICaseRepository>();
        _routingServiceMock = new Mock<ICaseRoutingService>();
        _auditServiceMock = new Mock<IAuditService>();
        _currentUserMock = new Mock<ICurrentUserContext>();
        _mapperMock = new Mock<IMapper>();

        _currentUserMock.SetupGet(x => x.UserId).Returns(1);
        _currentUserMock.SetupGet(x => x.CorrelationId).Returns("corr-1");

        _auditServiceMock.Setup(a => a.InsertAuditAsync(
                It.IsAny<EBOS.CRM.Application.Contracts.Requests.Services.AuditInsertRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        _handler = new RouteCaseCommandHandler(
            _repositoryMock.Object,
            _routingServiceMock.Object,
            _auditServiceMock.Object,
            _currentUserMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WhenRouteIsReturned_UpdatesCaseAndAudits()
    {
        var entity = new CaseEntity
        {
            Id = 5,
            TenantId = 1,
            Title = "Case",
            Status = CaseEntity.StatusOpen,
            Priority = CaseEntity.PriorityLow,
            QueueId = 1,
            OwnerUserId = 0
        };
        var request = new RouteCaseRequest(false);
        var response = new CaseResponse(entity.Id, entity.TenantId, entity.Title, entity.Description,
            entity.Status, entity.Priority, 10, 2, entity.SlaId, entity.DueAt, entity.ClosedAt, true);

        _repositoryMock.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _routingServiceMock.Setup(r => r.RouteAsync(entity, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RouteCaseResult(2, 10, "default-active-queue"));
        _mapperMock.Setup(m => m.Map<CaseResponse>(entity)).Returns(response);

        var result = await _handler.Handle(new RouteCaseCommand(entity.Id, request), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, entity.QueueId);
        Assert.Equal(10, entity.OwnerUserId);
        _repositoryMock.Verify(r => r.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _auditServiceMock.Verify(a => a.InsertAuditAsync(
            It.IsAny<EBOS.CRM.Application.Contracts.Requests.Services.AuditInsertRequest>(),
            It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCaseIsClosed_Throws()
    {
        var entity = new CaseEntity
        {
            Id = 6,
            TenantId = 1,
            Title = "Case",
            Status = CaseEntity.StatusClosed,
            Priority = CaseEntity.PriorityLow,
            QueueId = 1,
            OwnerUserId = 1,
            ClosedAt = DateTime.UtcNow
        };

        _repositoryMock.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(new RouteCaseCommand(entity.Id, new RouteCaseRequest()), CancellationToken.None));

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<CaseEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
