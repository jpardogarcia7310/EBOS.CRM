using EBOS.CRM.Application.Features.CRM.Service.Queue.Commands.AddQueue;
using EBOS.CRM.Application.Features.CRM.Service.Queue.Commands.AssignQueueDefaultOwner;
using EBOS.CRM.Application.Features.CRM.Service.Queue.Commands.ToggleQueue;
using EBOS.CRM.Application.Features.CRM.Service.Queue.Commands.UpdateQueue;
using EBOS.CRM.Application.Features.CRM.Service.Queue.Queries.GetAllQueues;
using EBOS.CRM.Application.Features.CRM.Service.Queue.Queries.GetQueueById;
using EBOS.CRM.Contracts.Requests.CRM.Service.Queue;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using Moq;
using QueueEntity = EBOS.CRM.Domain.Entities.CRM.Queue;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Queue;

public class QueueHandlersTests
{
    [Fact]
    public async Task AddQueue_WhenValid_AddsAndAudits()
    {
        var repo = new Mock<IQueueRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");

        audit.Setup(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var request = new AddQueueRequest(1, "Default", "DEF", true, null);
        var entity = new QueueEntity
        {
            Id = 1,
            TenantId = 1,
            Name = request.Name,
            Code = request.Code,
            IsActive = request.IsActive,
            DefaultOwnerUserId = request.DefaultOwnerUserId
        };
        var response = new QueueResponse(entity.Id, entity.TenantId, entity.Name, entity.Code, entity.IsActive,
            entity.DefaultOwnerUserId, true);

        mapper.Setup(m => m.Map<QueueEntity>(request)).Returns(entity);
        mapper.Setup(m => m.Map<QueueResponse>(entity)).Returns(response);

        var handler = new AddQueueCommandHandler(repo.Object, audit.Object, currentUser.Object, mapper.Object);
        var result = await handler.Handle(new AddQueueCommand(request), CancellationToken.None);

        Assert.NotNull(result);
        repo.Verify(r => r.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        audit.Verify(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateQueue_WhenNotFound_ReturnsNull()
    {
        var repo = new Mock<IQueueRepository>();
        var caseRepo = new Mock<ICaseRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        repo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((QueueEntity?)null);

        var handler = new UpdateQueueCommandHandler(repo.Object, caseRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);
        var request = new UpdateQueueRequest(99, 1, "Updated", "UPD", true, null);

        var result = await handler.Handle(new UpdateQueueCommand(99, request), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateQueue_WhenValid_UpdatesAndAudits()
    {
        var repo = new Mock<IQueueRepository>();
        var caseRepo = new Mock<ICaseRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        audit.Setup(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var entity = new QueueEntity { Id = 8, TenantId = 1, Name = "Q", Code = "Q", IsActive = true };
        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        caseRepo.Setup(r => r.CountOpenByQueueIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(0);

        var request = new UpdateQueueRequest(entity.Id, 1, "Updated", "UPD", true, 100);
        mapper.Setup(m => m.Map(request, entity)).Callback(() =>
        {
            entity.Name = request.Name;
            entity.Code = request.Code;
            entity.IsActive = request.IsActive;
            entity.DefaultOwnerUserId = request.DefaultOwnerUserId;
        });
        mapper.Setup(m => m.Map<QueueResponse>(entity)).Returns(
            new QueueResponse(entity.Id, entity.TenantId, entity.Name, entity.Code, entity.IsActive,
                entity.DefaultOwnerUserId, true));

        var handler = new UpdateQueueCommandHandler(repo.Object, caseRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var result = await handler.Handle(new UpdateQueueCommand(entity.Id, request), CancellationToken.None);

        Assert.NotNull(result);
        repo.Verify(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        audit.Verify(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ToggleQueue_WhenValid_ReturnsDto()
    {
        var repo = new Mock<IQueueRepository>();
        var caseRepo = new Mock<ICaseRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        var entity = new QueueEntity { Id = 7, TenantId = 1, Name = "Q", Code = "Q", IsActive = true };
        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        audit.Setup(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var response = new QueueResponse(entity.Id, entity.TenantId, entity.Name, entity.Code, false,
            entity.DefaultOwnerUserId, true);
        mapper.Setup(m => m.Map<QueueResponse>(entity)).Returns(response);

        var handler = new ToggleQueueCommandHandler(repo.Object, caseRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);
        var result = await handler.Handle(new ToggleQueueCommand(entity.Id, new ToggleQueueRequest(1, false)),
            CancellationToken.None);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task AssignDefaultOwner_WhenNotFound_ReturnsNull()
    {
        var repo = new Mock<IQueueRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        repo.Setup(r => r.GetByIdAsync(123, It.IsAny<CancellationToken>())).ReturnsAsync((QueueEntity?)null);

        var handler = new AssignQueueDefaultOwnerCommandHandler(repo.Object, audit.Object, currentUser.Object, mapper.Object);
        var request = new AssignQueueDefaultOwnerRequest(1, 10);

        var result = await handler.Handle(new AssignQueueDefaultOwnerCommand(123, request), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task AssignDefaultOwner_WhenValid_UpdatesAndAudits()
    {
        var repo = new Mock<IQueueRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        audit.Setup(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var entity = new QueueEntity { Id = 9, TenantId = 1, Name = "Q", Code = "Q", IsActive = true };
        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        mapper.Setup(m => m.Map<QueueResponse>(entity)).Returns(
            new QueueResponse(entity.Id, entity.TenantId, entity.Name, entity.Code, entity.IsActive,
                200, true));

        var handler = new AssignQueueDefaultOwnerCommandHandler(repo.Object, audit.Object, currentUser.Object, mapper.Object);
        var request = new AssignQueueDefaultOwnerRequest(1, 200);
        var result = await handler.Handle(new AssignQueueDefaultOwnerCommand(entity.Id, request), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(200, entity.DefaultOwnerUserId);
        repo.Verify(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        audit.Verify(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetQueueById_WhenExists_ReturnsDto()
    {
        var repo = new Mock<IQueueRepository>();
        var mapper = new Mock<IMapper>();
        var entity = new QueueEntity { Id = 5, TenantId = 1, Name = "Q", Code = "Q", IsActive = true };
        var response = new QueueResponse(entity.Id, entity.TenantId, entity.Name, entity.Code, entity.IsActive,
            entity.DefaultOwnerUserId, true);

        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        mapper.Setup(m => m.Map<QueueResponse>(entity)).Returns(response);

        var handler = new GetQueueByIdQueryHandler(repo.Object, mapper.Object);
        var result = await handler.Handle(new GetQueueByIdQuery(entity.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(entity.Id, result!.Id);
    }

    [Fact]
    public async Task GetAllQueues_ReturnsPagedResult()
    {
        var repo = new Mock<IQueueRepository>();
        var mapper = new Mock<IMapper>();
        var entities = new List<QueueEntity> { new() { Id = 1, TenantId = 1, Name = "Q", Code = "Q", IsActive = true } };
        var responses = new List<QueueResponse>
        {
            new(1, 1, "Q", "Q", true, null, true)
        };

        repo.Setup(r => r.GetAllPagedAsync(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(entities);
        repo.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        mapper.Setup(m => m.Map<IReadOnlyCollection<QueueResponse>>(entities)).Returns(responses);

        var handler = new GetAllQueuesQueryHandler(repo.Object, mapper.Object);
        var result = await handler.Handle(new GetAllQueuesQuery(1, 10), CancellationToken.None);

        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
    }
}
