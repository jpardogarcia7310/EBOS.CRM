using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AddCase;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AssignCaseQueue;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AssignCaseSla;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.CloseCase;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.AssignCaseOwner;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.DeleteCase;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.ReopenCase;
using EBOS.CRM.Application.Features.CRM.Service.Case.Commands.UpdateCase;
using EBOS.CRM.Application.Features.CRM.Service.Case.Queries.GetAllCases;
using EBOS.CRM.Application.Features.CRM.Service.Case.Queries.GetCaseById;
using EBOS.CRM.Contracts.Requests.CRM.Service.Case;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Exceptions;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using FluentAssertions;
using MapsterMapper;
using Moq;
using CaseEntity = EBOS.CRM.Domain.Entities.CRM.Case;
using QueueEntity = EBOS.CRM.Domain.Entities.CRM.Queue;
using SlaEntity = EBOS.CRM.Domain.Entities.CRM.Sla;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Case;

public class CaseHandlersTests
{
    private static AddCaseRequest BuildAddRequest(long queueId, long slaId) => new(
        TenantId: 1,
        Title: "Case",
        Description: "Desc",
        Status: CaseEntity.StatusOpen,
        Priority: CaseEntity.PriorityLow,
        OwnerUserId: 10,
        QueueId: queueId,
        SlaId: slaId,
        DueAt: null);

    [Fact]
    public async Task AddCase_WhenValid_AddsAndAudits()
    {
        var repo = new Mock<ICaseRepository>();
        var queueRepo = new Mock<IQueueRepository>();
        var slaRepo = new Mock<ISlaRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");

        audit.Setup(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var queue = new QueueEntity { Id = 2, TenantId = 1, IsActive = true };
        var sla = new SlaEntity { Id = 3, TenantId = 1, TargetMinutes = 30, IsActive = true };
        queueRepo.Setup(r => r.GetByIdAsync(queue.Id, It.IsAny<CancellationToken>())).ReturnsAsync(queue);
        slaRepo.Setup(r => r.GetByIdAsync(sla.Id, It.IsAny<CancellationToken>())).ReturnsAsync(sla);

        var request = BuildAddRequest(queue.Id, sla.Id);
        var entity = new CaseEntity
        {
            Id = 10,
            TenantId = 1,
            Title = request.Title,
            Status = request.Status,
            Priority = request.Priority,
            QueueId = request.QueueId,
            SlaId = request.SlaId,
            OwnerUserId = request.OwnerUserId,
            CreatedAt = DateTime.UtcNow
        };
        var response = new CaseResponse(entity.Id, entity.TenantId, entity.Title, entity.Description,
            entity.Status, entity.Priority, entity.OwnerUserId, entity.QueueId, entity.SlaId, entity.DueAt,
            entity.ClosedAt, true);

        mapper.Setup(m => m.Map<CaseEntity>(request)).Returns(entity);
        mapper.Setup(m => m.Map<CaseResponse>(entity)).Returns(response);

        var handler = new AddCaseCommandHandler(repo.Object, queueRepo.Object, slaRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var result = await handler.Handle(new AddCaseCommand(request), CancellationToken.None);

        Assert.NotNull(result);
        repo.Verify(r => r.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        audit.Verify(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddCase_WhenQueueTenantMismatch_Throws()
    {
        var repo = new Mock<ICaseRepository>();
        var queueRepo = new Mock<IQueueRepository>();
        var slaRepo = new Mock<ISlaRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        var queue = new QueueEntity { Id = 2, TenantId = 2, IsActive = true };
        var sla = new SlaEntity { Id = 3, TenantId = 1, TargetMinutes = 30, IsActive = true };

        queueRepo.Setup(r => r.GetByIdAsync(queue.Id, It.IsAny<CancellationToken>())).ReturnsAsync(queue);
        slaRepo.Setup(r => r.GetByIdAsync(sla.Id, It.IsAny<CancellationToken>())).ReturnsAsync(sla);

        var request = BuildAddRequest(queue.Id, sla.Id);
        var handler = new AddCaseCommandHandler(repo.Object, queueRepo.Object, slaRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var act = () => handler.Handle(new AddCaseCommand(request), CancellationToken.None);

        await FluentActions.Awaiting(act).Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Queue tenant mismatch.");
    }

    [Fact]
    public async Task AddCase_WhenSlaTenantMismatch_Throws()
    {
        var repo = new Mock<ICaseRepository>();
        var queueRepo = new Mock<IQueueRepository>();
        var slaRepo = new Mock<ISlaRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        var queue = new QueueEntity { Id = 2, TenantId = 1, IsActive = true };
        var sla = new SlaEntity { Id = 3, TenantId = 2, TargetMinutes = 30, IsActive = true };

        queueRepo.Setup(r => r.GetByIdAsync(queue.Id, It.IsAny<CancellationToken>())).ReturnsAsync(queue);
        slaRepo.Setup(r => r.GetByIdAsync(sla.Id, It.IsAny<CancellationToken>())).ReturnsAsync(sla);

        var request = BuildAddRequest(queue.Id, sla.Id);
        var handler = new AddCaseCommandHandler(repo.Object, queueRepo.Object, slaRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var act = () => handler.Handle(new AddCaseCommand(request), CancellationToken.None);

        await FluentActions.Awaiting(act).Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("SLA tenant mismatch.");
    }

    [Fact]
    public async Task AssignQueue_WhenNotFound_ReturnsNull()
    {
        var repo = new Mock<ICaseRepository>();
        var queueRepo = new Mock<IQueueRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        repo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((CaseEntity?)null);

        var handler = new AssignCaseQueueCommandHandler(repo.Object, queueRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var request = new AssignCaseQueueRequest(1, 5);
        var result = await handler.Handle(new AssignCaseQueueCommand(99, request), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task AssignQueue_WhenQueueNotFound_Throws()
    {
        var repo = new Mock<ICaseRepository>();
        var queueRepo = new Mock<IQueueRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        var entity = new CaseEntity
        {
            Id = 40,
            TenantId = 1,
            Title = "Case",
            Status = CaseEntity.StatusOpen,
            Priority = CaseEntity.PriorityLow,
            OwnerUserId = 10,
            QueueId = 2,
            SlaId = 3,
            CreatedAt = DateTime.UtcNow
        };

        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        queueRepo.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((QueueEntity?)null);

        var handler = new AssignCaseQueueCommandHandler(repo.Object, queueRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var request = new AssignCaseQueueRequest(1, 5);
        var act = () => handler.Handle(new AssignCaseQueueCommand(entity.Id, request), CancellationToken.None);

        await FluentActions.Awaiting(act).Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Queue not found.");
    }

    [Fact]
    public async Task AssignQueue_WhenQueueInactive_Throws()
    {
        var repo = new Mock<ICaseRepository>();
        var queueRepo = new Mock<IQueueRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        var entity = new CaseEntity
        {
            Id = 40,
            TenantId = 1,
            Title = "Case",
            Status = CaseEntity.StatusOpen,
            Priority = CaseEntity.PriorityLow,
            OwnerUserId = 10,
            QueueId = 2,
            SlaId = 3,
            CreatedAt = DateTime.UtcNow
        };

        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        queueRepo.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueueEntity { Id = 5, TenantId = 1, IsActive = false });

        var handler = new AssignCaseQueueCommandHandler(repo.Object, queueRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var request = new AssignCaseQueueRequest(1, 5);
        var act = () => handler.Handle(new AssignCaseQueueCommand(entity.Id, request), CancellationToken.None);

        await FluentActions.Awaiting(act).Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Queue is not active.");
    }

    [Fact]
    public async Task AssignQueue_WhenQueueTenantMismatch_Throws()
    {
        var repo = new Mock<ICaseRepository>();
        var queueRepo = new Mock<IQueueRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        var entity = new CaseEntity
        {
            Id = 40,
            TenantId = 1,
            Title = "Case",
            Status = CaseEntity.StatusOpen,
            Priority = CaseEntity.PriorityLow,
            OwnerUserId = 10,
            QueueId = 2,
            SlaId = 3,
            CreatedAt = DateTime.UtcNow
        };

        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        queueRepo.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueueEntity { Id = 5, TenantId = 2, IsActive = true });

        var handler = new AssignCaseQueueCommandHandler(repo.Object, queueRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var request = new AssignCaseQueueRequest(1, 5);
        var act = () => handler.Handle(new AssignCaseQueueCommand(entity.Id, request), CancellationToken.None);

        await FluentActions.Awaiting(act).Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Queue tenant mismatch.");
    }

    [Fact]
    public async Task AssignQueue_WhenValid_UpdatesAndAudits()
    {
        var repo = new Mock<ICaseRepository>();
        var queueRepo = new Mock<IQueueRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        audit.Setup(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var entity = new CaseEntity
        {
            Id = 41,
            TenantId = 1,
            Title = "Case",
            Status = CaseEntity.StatusOpen,
            Priority = CaseEntity.PriorityLow,
            OwnerUserId = 10,
            QueueId = 2,
            SlaId = 3,
            CreatedAt = DateTime.UtcNow
        };

        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        queueRepo.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueueEntity { Id = 5, TenantId = 1, IsActive = true });
        mapper.Setup(m => m.Map<CaseResponse>(It.IsAny<CaseEntity>())).Returns(
            new CaseResponse(entity.Id, entity.TenantId, entity.Title, entity.Description,
                entity.Status, entity.Priority, entity.OwnerUserId, 5, entity.SlaId,
                entity.DueAt, entity.ClosedAt, true));

        var handler = new AssignCaseQueueCommandHandler(repo.Object, queueRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var request = new AssignCaseQueueRequest(1, 5);
        var result = await handler.Handle(new AssignCaseQueueCommand(entity.Id, request), CancellationToken.None);

        Assert.NotNull(result);
        entity.QueueId.Should().Be(5);
        audit.Verify(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AssignSla_WhenSlaTenantMismatch_Throws()
    {
        var repo = new Mock<ICaseRepository>();
        var slaRepo = new Mock<ISlaRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        var entity = new CaseEntity
        {
            Id = 41,
            TenantId = 1,
            Title = "Case",
            Status = CaseEntity.StatusOpen,
            Priority = CaseEntity.PriorityLow,
            OwnerUserId = 10,
            QueueId = 2,
            SlaId = 3,
            CreatedAt = DateTime.UtcNow
        };

        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        slaRepo.Setup(r => r.GetByIdAsync(6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SlaEntity { Id = 6, TenantId = 2, TargetMinutes = 30 });

        var handler = new AssignCaseSlaCommandHandler(repo.Object, slaRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var request = new AssignCaseSlaRequest(1, 6);
        var act = () => handler.Handle(new AssignCaseSlaCommand(entity.Id, request), CancellationToken.None);

        await FluentActions.Awaiting(act).Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("SLA tenant mismatch.");
    }

    [Fact]
    public async Task AssignSla_WhenValid_UpdatesAndAudits()
    {
        var repo = new Mock<ICaseRepository>();
        var slaRepo = new Mock<ISlaRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        audit.Setup(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var entity = new CaseEntity
        {
            Id = 42,
            TenantId = 1,
            Title = "Case",
            Status = CaseEntity.StatusOpen,
            Priority = CaseEntity.PriorityLow,
            OwnerUserId = 10,
            QueueId = 2,
            SlaId = 3,
            CreatedAt = DateTime.UtcNow
        };

        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        slaRepo.Setup(r => r.GetByIdAsync(6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SlaEntity { Id = 6, TenantId = 1, TargetMinutes = 30 });
        mapper.Setup(m => m.Map<CaseResponse>(It.IsAny<CaseEntity>())).Returns(
            new CaseResponse(entity.Id, entity.TenantId, entity.Title, entity.Description,
                entity.Status, entity.Priority, entity.OwnerUserId, entity.QueueId, 6,
                entity.DueAt, entity.ClosedAt, true));

        var handler = new AssignCaseSlaCommandHandler(repo.Object, slaRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var request = new AssignCaseSlaRequest(1, 6);
        var result = await handler.Handle(new AssignCaseSlaCommand(entity.Id, request), CancellationToken.None);

        Assert.NotNull(result);
        entity.SlaId.Should().Be(6);
        audit.Verify(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
    [Fact]
    public async Task UpdateCase_WhenNotFound_ReturnsNull()
    {
        var repo = new Mock<ICaseRepository>();
        var queueRepo = new Mock<IQueueRepository>();
        var slaRepo = new Mock<ISlaRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        repo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((CaseEntity?)null);

        var handler = new UpdateCaseCommandHandler(repo.Object, queueRepo.Object, slaRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var request = new UpdateCaseRequest(
            Id: 99,
            TenantId: 1,
            Title: "Updated",
            Description: null,
            Status: CaseEntity.StatusOpen,
            Priority: CaseEntity.PriorityLow,
            OwnerUserId: 1,
            QueueId: 2,
            SlaId: 3,
            DueAt: null);

        var result = await handler.Handle(new UpdateCaseCommand(99, request), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateCase_WhenQueueInactive_Throws()
    {
        var repo = new Mock<ICaseRepository>();
        var queueRepo = new Mock<IQueueRepository>();
        var slaRepo = new Mock<ISlaRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        var entity = new CaseEntity
        {
            Id = 10,
            TenantId = 1,
            Title = "Case",
            Status = CaseEntity.StatusOpen,
            Priority = CaseEntity.PriorityLow,
            OwnerUserId = 10,
            QueueId = 2,
            SlaId = 3,
            CreatedAt = DateTime.UtcNow
        };

        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        queueRepo.Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueueEntity { Id = 2, TenantId = 1, IsActive = false });
        slaRepo.Setup(r => r.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SlaEntity { Id = 3, TenantId = 1, TargetMinutes = 30 });

        var handler = new UpdateCaseCommandHandler(repo.Object, queueRepo.Object, slaRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var request = new UpdateCaseRequest(
            Id: entity.Id,
            TenantId: 1,
            Title: "Updated",
            Description: null,
            Status: CaseEntity.StatusOpen,
            Priority: CaseEntity.PriorityLow,
            OwnerUserId: 10,
            QueueId: 2,
            SlaId: 3,
            DueAt: null);

        var act = () => handler.Handle(new UpdateCaseCommand(entity.Id, request), CancellationToken.None);

        await FluentActions.Awaiting(act).Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Queue is not active.");
    }

    [Fact]
    public async Task UpdateCase_WhenQueueTenantMismatch_Throws()
    {
        var repo = new Mock<ICaseRepository>();
        var queueRepo = new Mock<IQueueRepository>();
        var slaRepo = new Mock<ISlaRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        var entity = new CaseEntity
        {
            Id = 13,
            TenantId = 1,
            Title = "Case",
            Status = CaseEntity.StatusOpen,
            Priority = CaseEntity.PriorityLow,
            OwnerUserId = 10,
            QueueId = 2,
            SlaId = 3,
            CreatedAt = DateTime.UtcNow
        };

        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        queueRepo.Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueueEntity { Id = 2, TenantId = 2, IsActive = true });
        slaRepo.Setup(r => r.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SlaEntity { Id = 3, TenantId = 1, TargetMinutes = 30 });

        var handler = new UpdateCaseCommandHandler(repo.Object, queueRepo.Object, slaRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var request = new UpdateCaseRequest(
            Id: entity.Id,
            TenantId: 1,
            Title: "Updated",
            Description: null,
            Status: CaseEntity.StatusOpen,
            Priority: CaseEntity.PriorityLow,
            OwnerUserId: 10,
            QueueId: 2,
            SlaId: 3,
            DueAt: null);

        var act = () => handler.Handle(new UpdateCaseCommand(entity.Id, request), CancellationToken.None);

        await FluentActions.Awaiting(act).Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Queue tenant mismatch.");
    }

    [Fact]
    public async Task UpdateCase_WhenSlaNotFound_Throws()
    {
        var repo = new Mock<ICaseRepository>();
        var queueRepo = new Mock<IQueueRepository>();
        var slaRepo = new Mock<ISlaRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        var entity = new CaseEntity
        {
            Id = 14,
            TenantId = 1,
            Title = "Case",
            Status = CaseEntity.StatusOpen,
            Priority = CaseEntity.PriorityLow,
            OwnerUserId = 10,
            QueueId = 2,
            SlaId = 3,
            CreatedAt = DateTime.UtcNow
        };

        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        queueRepo.Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueueEntity { Id = 2, TenantId = 1, IsActive = true });
        slaRepo.Setup(r => r.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SlaEntity?)null);

        var handler = new UpdateCaseCommandHandler(repo.Object, queueRepo.Object, slaRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var request = new UpdateCaseRequest(
            Id: entity.Id,
            TenantId: 1,
            Title: "Updated",
            Description: null,
            Status: CaseEntity.StatusOpen,
            Priority: CaseEntity.PriorityLow,
            OwnerUserId: 10,
            QueueId: 2,
            SlaId: 3,
            DueAt: null);

        var act = () => handler.Handle(new UpdateCaseCommand(entity.Id, request), CancellationToken.None);

        await FluentActions.Awaiting(act).Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("SLA not found.");
    }

    [Fact]
    public async Task UpdateCase_WhenSlaTenantMismatch_Throws()
    {
        var repo = new Mock<ICaseRepository>();
        var queueRepo = new Mock<IQueueRepository>();
        var slaRepo = new Mock<ISlaRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        var entity = new CaseEntity
        {
            Id = 11,
            TenantId = 1,
            Title = "Case",
            Status = CaseEntity.StatusOpen,
            Priority = CaseEntity.PriorityLow,
            OwnerUserId = 10,
            QueueId = 2,
            SlaId = 3,
            CreatedAt = DateTime.UtcNow
        };

        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        queueRepo.Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueueEntity { Id = 2, TenantId = 1, IsActive = true });
        slaRepo.Setup(r => r.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SlaEntity { Id = 3, TenantId = 2, TargetMinutes = 30 });

        var handler = new UpdateCaseCommandHandler(repo.Object, queueRepo.Object, slaRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var request = new UpdateCaseRequest(
            Id: entity.Id,
            TenantId: 1,
            Title: "Updated",
            Description: null,
            Status: CaseEntity.StatusOpen,
            Priority: CaseEntity.PriorityLow,
            OwnerUserId: 10,
            QueueId: 2,
            SlaId: 3,
            DueAt: null);

        var act = () => handler.Handle(new UpdateCaseCommand(entity.Id, request), CancellationToken.None);

        await FluentActions.Awaiting(act).Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("SLA tenant mismatch.");
    }

    [Fact]
    public async Task UpdateCase_WhenValid_UpdatesAndAudits()
    {
        var repo = new Mock<ICaseRepository>();
        var queueRepo = new Mock<IQueueRepository>();
        var slaRepo = new Mock<ISlaRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        audit.Setup(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var entity = new CaseEntity
        {
            Id = 12,
            TenantId = 1,
            Title = "Case",
            Status = CaseEntity.StatusOpen,
            Priority = CaseEntity.PriorityLow,
            OwnerUserId = 10,
            QueueId = 2,
            SlaId = 3,
            CreatedAt = DateTime.UtcNow
        };

        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        queueRepo.Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new QueueEntity { Id = 2, TenantId = 1, IsActive = true });
        slaRepo.Setup(r => r.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SlaEntity { Id = 3, TenantId = 1, TargetMinutes = 30 });

        mapper.Setup(m => m.Map<CaseResponse>(It.IsAny<CaseEntity>())).Returns(
            new CaseResponse(entity.Id, entity.TenantId, "Updated", null,
                CaseEntity.StatusInProgress, CaseEntity.PriorityLow, entity.OwnerUserId,
                entity.QueueId, entity.SlaId, entity.DueAt, entity.ClosedAt, true));

        var handler = new UpdateCaseCommandHandler(repo.Object, queueRepo.Object, slaRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var request = new UpdateCaseRequest(
            Id: entity.Id,
            TenantId: 1,
            Title: "Updated",
            Description: null,
            Status: CaseEntity.StatusInProgress,
            Priority: CaseEntity.PriorityLow,
            OwnerUserId: 10,
            QueueId: 2,
            SlaId: 3,
            DueAt: null);

        var result = await handler.Handle(new UpdateCaseCommand(entity.Id, request), CancellationToken.None);

        Assert.NotNull(result);
        repo.Verify(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        audit.Verify(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CloseCase_WhenNotFound_ReturnsNull()
    {
        var repo = new Mock<ICaseRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        repo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((CaseEntity?)null);

        var handler = new CloseCaseCommandHandler(repo.Object, audit.Object, currentUser.Object, mapper.Object);
        var request = new CloseCaseRequest(1, DateTime.UtcNow);

        var result = await handler.Handle(new CloseCaseCommand(999, request), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task CloseCase_WhenValid_ClosesAndAudits()
    {
        var repo = new Mock<ICaseRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        audit.Setup(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var entity = new CaseEntity
        {
            Id = 20,
            TenantId = 1,
            Title = "Case",
            Status = CaseEntity.StatusResolved,
            Priority = CaseEntity.PriorityLow,
            OwnerUserId = 10,
            QueueId = 2,
            SlaId = 3,
            CreatedAt = DateTime.UtcNow
        };

        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        mapper.Setup(m => m.Map<CaseResponse>(It.IsAny<CaseEntity>())).Returns(
            new CaseResponse(entity.Id, entity.TenantId, entity.Title, entity.Description,
                CaseEntity.StatusClosed, entity.Priority, entity.OwnerUserId, entity.QueueId, entity.SlaId,
                entity.DueAt, entity.ClosedAt, true));

        var handler = new CloseCaseCommandHandler(repo.Object, audit.Object, currentUser.Object, mapper.Object);
        var request = new CloseCaseRequest(1, DateTime.UtcNow);

        var result = await handler.Handle(new CloseCaseCommand(entity.Id, request), CancellationToken.None);

        Assert.NotNull(result);
        entity.Status.Should().Be(CaseEntity.StatusClosed);
        repo.Verify(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        audit.Verify(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CloseCase_WhenAlreadyClosed_Throws()
    {
        var repo = new Mock<ICaseRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        var entity = new CaseEntity
        {
            Id = 21,
            TenantId = 1,
            Title = "Case",
            Status = CaseEntity.StatusClosed,
            Priority = CaseEntity.PriorityLow,
            OwnerUserId = 10,
            QueueId = 2,
            SlaId = 3,
            ClosedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var handler = new CloseCaseCommandHandler(repo.Object, audit.Object, currentUser.Object, mapper.Object);
        var request = new CloseCaseRequest(1, DateTime.UtcNow);

        var act = () => handler.Handle(new CloseCaseCommand(entity.Id, request), CancellationToken.None);

        await FluentActions.Awaiting(act).Should().ThrowAsync<DomainRuleViolationException>()
            .WithMessage("Case is already closed.");
    }

    [Fact]
    public async Task ReopenCase_WhenNotFound_ReturnsNull()
    {
        var repo = new Mock<ICaseRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        repo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((CaseEntity?)null);

        var handler = new ReopenCaseCommandHandler(repo.Object, audit.Object, currentUser.Object, mapper.Object);
        var request = new ReopenCaseRequest(1);

        var result = await handler.Handle(new ReopenCaseCommand(999, request), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task ReopenCase_WhenValid_ReopensAndAudits()
    {
        var repo = new Mock<ICaseRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        audit.Setup(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var entity = new CaseEntity
        {
            Id = 30,
            TenantId = 1,
            Title = "Case",
            Status = CaseEntity.StatusClosed,
            Priority = CaseEntity.PriorityLow,
            OwnerUserId = 10,
            QueueId = 2,
            SlaId = 3,
            ClosedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        mapper.Setup(m => m.Map<CaseResponse>(It.IsAny<CaseEntity>())).Returns(
            new CaseResponse(entity.Id, entity.TenantId, entity.Title, entity.Description,
                CaseEntity.StatusReopened, entity.Priority, entity.OwnerUserId, entity.QueueId, entity.SlaId,
                entity.DueAt, entity.ClosedAt, true));

        var handler = new ReopenCaseCommandHandler(repo.Object, audit.Object, currentUser.Object, mapper.Object);
        var request = new ReopenCaseRequest(1);

        var result = await handler.Handle(new ReopenCaseCommand(entity.Id, request), CancellationToken.None);

        Assert.NotNull(result);
        entity.Status.Should().Be(CaseEntity.StatusReopened);
        repo.Verify(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        audit.Verify(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReopenCase_WhenNotClosed_Throws()
    {
        var repo = new Mock<ICaseRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        var entity = new CaseEntity
        {
            Id = 31,
            TenantId = 1,
            Title = "Case",
            Status = CaseEntity.StatusOpen,
            Priority = CaseEntity.PriorityLow,
            OwnerUserId = 10,
            QueueId = 2,
            SlaId = 3,
            CreatedAt = DateTime.UtcNow
        };

        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var handler = new ReopenCaseCommandHandler(repo.Object, audit.Object, currentUser.Object, mapper.Object);
        var request = new ReopenCaseRequest(1);

        var act = () => handler.Handle(new ReopenCaseCommand(entity.Id, request), CancellationToken.None);

        await FluentActions.Awaiting(act).Should().ThrowAsync<DomainRuleViolationException>()
            .WithMessage("Case is not closed.");
    }

    [Fact]
    public async Task DeleteCase_WhenNotFound_ReturnsFalse()
    {
        var repo = new Mock<ICaseRepository>();
        repo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((CaseEntity?)null);

        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        var handler = new DeleteCaseCommandHandler(repo.Object, audit.Object, currentUser.Object);
        var result = await handler.Handle(new DeleteCaseCommand(99), CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteCase_WhenFound_Deletes()
    {
        var repo = new Mock<ICaseRepository>();
        var entity = new CaseEntity { Id = 60, TenantId = 1 };
        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);

        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();
        audit.Setup(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));
        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");

        var handler = new DeleteCaseCommandHandler(repo.Object, audit.Object, currentUser.Object);
        var result = await handler.Handle(new DeleteCaseCommand(entity.Id), CancellationToken.None);

        Assert.True(result);
        repo.Verify(r => r.DeleteAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCaseById_WhenExists_ReturnsDto()
    {
        var repo = new Mock<ICaseRepository>();
        var mapper = new Mock<IMapper>();
        var entity = new CaseEntity { Id = 5, TenantId = 1, Title = "Case", Status = CaseEntity.StatusOpen,
            Priority = CaseEntity.PriorityLow, QueueId = 1, SlaId = 1, OwnerUserId = 1, CreatedAt = DateTime.UtcNow };
        var response = new CaseResponse(entity.Id, entity.TenantId, entity.Title, entity.Description,
            entity.Status, entity.Priority, entity.OwnerUserId, entity.QueueId, entity.SlaId, entity.DueAt,
            entity.ClosedAt, true);

        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        mapper.Setup(m => m.Map<CaseResponse>(entity)).Returns(response);

        var handler = new GetCaseByIdQueryHandler(repo.Object, mapper.Object);
        var result = await handler.Handle(new GetCaseByIdQuery(entity.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(entity.Id, result!.Id);
    }

    [Fact]
    public async Task GetAllCases_ReturnsPagedResult()
    {
        var repo = new Mock<ICaseRepository>();
        var mapper = new Mock<IMapper>();
        var entities = new List<CaseEntity>
        {
            new() { Id = 1, TenantId = 1, Title = "A", Status = CaseEntity.StatusOpen, Priority = CaseEntity.PriorityLow,
                QueueId = 1, SlaId = 1, OwnerUserId = 1, CreatedAt = DateTime.UtcNow }
        };
        var responses = new List<CaseResponse>
        {
            new(1, 1, "A", null, CaseEntity.StatusOpen, CaseEntity.PriorityLow, 1, 1, 1, null, null, true)
        };

        repo.Setup(r => r.GetAllPagedAsync(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(entities);
        repo.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        mapper.Setup(m => m.Map<IReadOnlyCollection<CaseResponse>>(entities)).Returns(responses);

        var handler = new GetAllCasesQueryHandler(repo.Object, mapper.Object);
        var result = await handler.Handle(new GetAllCasesQuery(1, 10), CancellationToken.None);

        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
    }
}

