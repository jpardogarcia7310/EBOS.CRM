using EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Commands.AddCaseActivity;
using EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Commands.DeleteCaseActivity;
using EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Commands.UpdateCaseActivity;
using EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Queries.GetAllCaseActivities;
using EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Queries.GetCaseActivitiesByCaseId;
using EBOS.CRM.Application.Features.CRM.Service.CaseActivity.Queries.GetCaseActivityById;
using EBOS.CRM.Contracts.Requests.CRM.Service.CaseActivity;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using FluentAssertions;
using MapsterMapper;
using Moq;
using CaseActivityEntity = EBOS.CRM.Domain.Entities.CRM.CaseActivity;
using CaseEntity = EBOS.CRM.Domain.Entities.CRM.Case;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.CaseActivity;

public class CaseActivityHandlersTests
{
    [Fact]
    public async Task AddCaseActivity_WhenValid_AddsAndAudits()
    {
        var repo = new Mock<ICaseActivityRepository>();
        var caseRepo = new Mock<ICaseRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        audit.Setup(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var caseEntity = new CaseEntity
        {
            Id = 10,
            TenantId = 1,
            Title = "Case",
            Status = CaseEntity.StatusOpen,
            Priority = CaseEntity.PriorityLow,
            OwnerUserId = 1,
            QueueId = 1,
            SlaId = 1,
            CreatedAt = DateTime.UtcNow
        };
        caseRepo.Setup(r => r.GetByIdAsync(caseEntity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(caseEntity);

        var request = new AddCaseActivityRequest(1, caseEntity.Id, "Activity", "Desc",
            CaseActivityEntity.StatusOpen);
        var entity = new CaseActivityEntity
        {
            Id = 5,
            TenantId = 1,
            CaseId = caseEntity.Id,
            Title = request.Title,
            Description = request.Description,
            Status = request.Status,
            CreatedAt = DateTime.UtcNow
        };
        var response = new CaseActivityResponse(entity.Id, entity.TenantId, entity.CaseId,
            entity.Title, entity.Description, entity.Status, true);

        mapper.Setup(m => m.Map<CaseActivityEntity>(request)).Returns(entity);
        mapper.Setup(m => m.Map<CaseActivityResponse>(entity)).Returns(response);

        var handler = new AddCaseActivityCommandHandler(repo.Object, caseRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var result = await handler.Handle(new AddCaseActivityCommand(request), CancellationToken.None);

        Assert.NotNull(result);
        repo.Verify(r => r.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        audit.Verify(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddCaseActivity_WhenCaseNotFound_Throws()
    {
        var repo = new Mock<ICaseActivityRepository>();
        var caseRepo = new Mock<ICaseRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        caseRepo.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CaseEntity?)null);

        var handler = new AddCaseActivityCommandHandler(repo.Object, caseRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);
        var request = new AddCaseActivityRequest(1, 10, "Activity", "Desc",
            CaseActivityEntity.StatusOpen);

        var act = () => handler.Handle(new AddCaseActivityCommand(request), CancellationToken.None);

        await FluentActions.Awaiting(act).Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Case not found.");
    }

    [Fact]
    public async Task AddCaseActivity_WhenCaseTenantMismatch_Throws()
    {
        var repo = new Mock<ICaseActivityRepository>();
        var caseRepo = new Mock<ICaseRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        var caseEntity = new CaseEntity
        {
            Id = 10,
            TenantId = 2,
            Title = "Case",
            Status = CaseEntity.StatusOpen,
            Priority = CaseEntity.PriorityLow,
            OwnerUserId = 1,
            QueueId = 1,
            SlaId = 1,
            CreatedAt = DateTime.UtcNow
        };
        caseRepo.Setup(r => r.GetByIdAsync(caseEntity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(caseEntity);

        var handler = new AddCaseActivityCommandHandler(repo.Object, caseRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);
        var request = new AddCaseActivityRequest(1, caseEntity.Id, "Activity", "Desc",
            CaseActivityEntity.StatusOpen);

        var act = () => handler.Handle(new AddCaseActivityCommand(request), CancellationToken.None);

        await FluentActions.Awaiting(act).Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Case tenant mismatch.");
    }

    [Fact]
    public async Task AddCaseActivity_WhenCaseClosed_Throws()
    {
        var repo = new Mock<ICaseActivityRepository>();
        var caseRepo = new Mock<ICaseRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        var caseEntity = new CaseEntity
        {
            Id = 10,
            TenantId = 1,
            Title = "Case",
            Status = CaseEntity.StatusClosed,
            Priority = CaseEntity.PriorityLow,
            OwnerUserId = 1,
            QueueId = 1,
            SlaId = 1,
            ClosedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        caseRepo.Setup(r => r.GetByIdAsync(caseEntity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(caseEntity);

        var handler = new AddCaseActivityCommandHandler(repo.Object, caseRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);
        var request = new AddCaseActivityRequest(1, caseEntity.Id, "Activity", "Desc",
            CaseActivityEntity.StatusOpen);

        var act = () => handler.Handle(new AddCaseActivityCommand(request), CancellationToken.None);

        await FluentActions.Awaiting(act).Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot add activities to a closed case.");
    }

    [Fact]
    public async Task UpdateCaseActivity_WhenNotFound_ReturnsNull()
    {
        var repo = new Mock<ICaseActivityRepository>();
        var caseRepo = new Mock<ICaseRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        repo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CaseActivityEntity?)null);

        var handler = new UpdateCaseActivityCommandHandler(repo.Object, caseRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var request = new UpdateCaseActivityRequest(99, 1, 10, "Activity", "Desc",
            CaseActivityEntity.StatusOpen);
        var result = await handler.Handle(new UpdateCaseActivityCommand(99, request), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateCaseActivity_WhenCaseNotFound_Throws()
    {
        var repo = new Mock<ICaseActivityRepository>();
        var caseRepo = new Mock<ICaseRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        var entity = new CaseActivityEntity
        {
            Id = 11,
            TenantId = 1,
            CaseId = 10,
            Title = "Activity",
            Status = CaseActivityEntity.StatusOpen,
            CreatedAt = DateTime.UtcNow
        };
        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        caseRepo.Setup(r => r.GetByIdAsync(entity.CaseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CaseEntity?)null);

        var handler = new UpdateCaseActivityCommandHandler(repo.Object, caseRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var request = new UpdateCaseActivityRequest(entity.Id, 1, entity.CaseId, "Activity", "Desc",
            CaseActivityEntity.StatusOpen);

        var act = () => handler.Handle(new UpdateCaseActivityCommand(entity.Id, request), CancellationToken.None);
        await FluentActions.Awaiting(act).Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Case not found.");
    }

    [Fact]
    public async Task UpdateCaseActivity_WhenCaseTenantMismatch_Throws()
    {
        var repo = new Mock<ICaseActivityRepository>();
        var caseRepo = new Mock<ICaseRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        var entity = new CaseActivityEntity
        {
            Id = 11,
            TenantId = 1,
            CaseId = 10,
            Title = "Activity",
            Status = CaseActivityEntity.StatusOpen,
            CreatedAt = DateTime.UtcNow
        };
        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        caseRepo.Setup(r => r.GetByIdAsync(entity.CaseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CaseEntity
            {
                Id = entity.CaseId,
                TenantId = 2,
                Title = "Case",
                Status = CaseEntity.StatusOpen,
                Priority = CaseEntity.PriorityLow,
                OwnerUserId = 1,
                QueueId = 1,
                SlaId = 1,
                CreatedAt = DateTime.UtcNow
            });

        var handler = new UpdateCaseActivityCommandHandler(repo.Object, caseRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var request = new UpdateCaseActivityRequest(entity.Id, 1, entity.CaseId, "Activity", "Desc",
            CaseActivityEntity.StatusOpen);

        var act = () => handler.Handle(new UpdateCaseActivityCommand(entity.Id, request), CancellationToken.None);
        await FluentActions.Awaiting(act).Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Case tenant mismatch.");
    }

    [Fact]
    public async Task UpdateCaseActivity_WhenCaseIdChanges_Throws()
    {
        var repo = new Mock<ICaseActivityRepository>();
        var caseRepo = new Mock<ICaseRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        var entity = new CaseActivityEntity
        {
            Id = 11,
            TenantId = 1,
            CaseId = 10,
            Title = "Activity",
            Status = CaseActivityEntity.StatusOpen,
            CreatedAt = DateTime.UtcNow
        };
        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        caseRepo.Setup(r => r.GetByIdAsync(entity.CaseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CaseEntity
            {
                Id = entity.CaseId,
                TenantId = 1,
                Title = "Case",
                Status = CaseEntity.StatusOpen,
                Priority = CaseEntity.PriorityLow,
                OwnerUserId = 1,
                QueueId = 1,
                SlaId = 1,
                CreatedAt = DateTime.UtcNow
            });

        var handler = new UpdateCaseActivityCommandHandler(repo.Object, caseRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var request = new UpdateCaseActivityRequest(entity.Id, 1, 99, "Activity", "Desc",
            CaseActivityEntity.StatusOpen);

        var act = () => handler.Handle(new UpdateCaseActivityCommand(entity.Id, request), CancellationToken.None);
        await FluentActions.Awaiting(act).Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("CaseId cannot be changed for an activity.");
    }

    [Fact]
    public async Task UpdateCaseActivity_WhenValid_UpdatesAndAudits()
    {
        var repo = new Mock<ICaseActivityRepository>();
        var caseRepo = new Mock<ICaseRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        audit.Setup(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var entity = new CaseActivityEntity
        {
            Id = 12,
            TenantId = 1,
            CaseId = 10,
            Title = "Activity",
            Status = CaseActivityEntity.StatusOpen,
            CreatedAt = DateTime.UtcNow
        };
        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        caseRepo.Setup(r => r.GetByIdAsync(entity.CaseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CaseEntity
            {
                Id = entity.CaseId,
                TenantId = 1,
                Title = "Case",
                Status = CaseEntity.StatusOpen,
                Priority = CaseEntity.PriorityLow,
                OwnerUserId = 1,
                QueueId = 1,
                SlaId = 1,
                CreatedAt = DateTime.UtcNow
            });

        var request = new UpdateCaseActivityRequest(entity.Id, 1, entity.CaseId, "Activity Updated", "Desc",
            CaseActivityEntity.StatusInProgress);
        mapper.Setup(m => m.Map(request, entity)).Callback(() =>
        {
            entity.Title = request.Title;
            entity.Description = request.Description;
            entity.Status = request.Status;
        });
        mapper.Setup(m => m.Map<CaseActivityResponse>(entity)).Returns(
            new CaseActivityResponse(entity.Id, entity.TenantId, entity.CaseId, entity.Title,
                entity.Description, entity.Status, true));

        var handler = new UpdateCaseActivityCommandHandler(repo.Object, caseRepo.Object,
            audit.Object, currentUser.Object, mapper.Object);

        var result = await handler.Handle(new UpdateCaseActivityCommand(entity.Id, request), CancellationToken.None);

        Assert.NotNull(result);
        repo.Verify(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        audit.Verify(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteCaseActivity_WhenNotFound_ReturnsFalse()
    {
        var repo = new Mock<ICaseActivityRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();

        repo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CaseActivityEntity?)null);

        var handler = new DeleteCaseActivityCommandHandler(repo.Object, audit.Object, currentUser.Object);
        var result = await handler.Handle(new DeleteCaseActivityCommand(99), CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteCaseActivity_WhenFound_Deletes()
    {
        var repo = new Mock<ICaseActivityRepository>();
        var audit = new Mock<IAuditService>();
        var currentUser = new Mock<ICurrentUserContext>();

        var entity = new CaseActivityEntity
        {
            Id = 13,
            TenantId = 1,
            CaseId = 10,
            Title = "Activity",
            Status = CaseActivityEntity.StatusOpen,
            CreatedAt = DateTime.UtcNow
        };
        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        audit.Setup(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));
        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");

        var handler = new DeleteCaseActivityCommandHandler(repo.Object, audit.Object, currentUser.Object);
        var result = await handler.Handle(new DeleteCaseActivityCommand(entity.Id), CancellationToken.None);

        Assert.True(result);
        repo.Verify(r => r.DeleteAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCaseActivityById_WhenExists_ReturnsDto()
    {
        var repo = new Mock<ICaseActivityRepository>();
        var mapper = new Mock<IMapper>();
        var entity = new CaseActivityEntity
        {
            Id = 20,
            TenantId = 1,
            CaseId = 10,
            Title = "Activity",
            Status = CaseActivityEntity.StatusOpen,
            CreatedAt = DateTime.UtcNow
        };
        var response = new CaseActivityResponse(entity.Id, entity.TenantId, entity.CaseId,
            entity.Title, entity.Description, entity.Status, true);

        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        mapper.Setup(m => m.Map<CaseActivityResponse>(entity)).Returns(response);

        var handler = new GetCaseActivityByIdQueryHandler(repo.Object, mapper.Object);
        var result = await handler.Handle(new GetCaseActivityByIdQuery(entity.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(entity.Id, result!.Id);
    }

    [Fact]
    public async Task GetAllCaseActivities_ReturnsPagedResult()
    {
        var repo = new Mock<ICaseActivityRepository>();
        var mapper = new Mock<IMapper>();
        var entities = new List<CaseActivityEntity>
        {
            new() { Id = 1, TenantId = 1, CaseId = 10, Title = "A", Status = CaseActivityEntity.StatusOpen }
        };
        var responses = new List<CaseActivityResponse>
        {
            new(1, 1, 10, "A", null, CaseActivityEntity.StatusOpen, true)
        };

        repo.Setup(r => r.GetAllPagedAsync(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(entities);
        repo.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        mapper.Setup(m => m.Map<IReadOnlyCollection<CaseActivityResponse>>(entities)).Returns(responses);

        var handler = new GetAllCaseActivitiesQueryHandler(repo.Object, mapper.Object);
        var result = await handler.Handle(new GetAllCaseActivitiesQuery(1, 10), CancellationToken.None);

        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task GetCaseActivitiesByCaseId_ReturnsPagedResult()
    {
        var repo = new Mock<ICaseActivityRepository>();
        var mapper = new Mock<IMapper>();
        var entities = new List<CaseActivityEntity>
        {
            new() { Id = 1, TenantId = 1, CaseId = 10, Title = "A", Status = CaseActivityEntity.StatusOpen }
        };
        var responses = new List<CaseActivityResponse>
        {
            new(1, 1, 10, "A", null, CaseActivityEntity.StatusOpen, true)
        };

        repo.Setup(r => r.GetAllByCaseIdPagedAsync(10, 1, 10, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entities);
        repo.Setup(r => r.CountByCaseIdAsync(10, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        mapper.Setup(m => m.Map<IReadOnlyCollection<CaseActivityResponse>>(entities)).Returns(responses);

        var handler = new GetCaseActivitiesByCaseIdQueryHandler(repo.Object, mapper.Object);
        var result = await handler.Handle(new GetCaseActivitiesByCaseIdQuery(10, 1, 10, null, null, null),
            CancellationToken.None);

        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
    }
}
