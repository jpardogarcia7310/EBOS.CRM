using EBOS.CRM.Application.Features.CRM.Service.Sla.Commands.AddSla;
using EBOS.CRM.Application.Features.CRM.Service.Sla.Commands.ToggleSla;
using EBOS.CRM.Application.Features.CRM.Service.Sla.Commands.UpdateSla;
using EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.CheckCaseSla;
using EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.GetAllSlas;
using EBOS.CRM.Application.Features.CRM.Service.Sla.Queries.GetSlaById;
using EBOS.CRM.Contracts.Requests.CRM.Service.Sla;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.CRM;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Repositories.CRM;
using EBOS.CRM.Domain.Interfaces.Services;
using MapsterMapper;
using Moq;
using CaseEntity = EBOS.CRM.Domain.Entities.CRM.Case;
using SlaEntity = EBOS.CRM.Domain.Entities.CRM.Sla;

namespace EBOS.CRM.ApiTests.Application.Features.CRM.Service.Sla;

public class SlaHandlersTests
{
    [Fact]
    public async Task AddSla_WhenValid_AddsAndAudits()
    {
        var repo = new Mock<ISlaRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");

        audit.Setup(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var request = new AddSlaRequest(1, "Standard", 60, 30, null, null, true);
        var entity = new SlaEntity
        {
            Id = 1,
            TenantId = 1,
            Name = request.Name,
            TargetMinutes = request.TargetMinutes,
            WarningMinutes = request.WarningMinutes,
            IsActive = request.IsActive
        };
        var response = new SlaResponse(entity.Id, entity.TenantId, entity.Name, entity.TargetMinutes,
            entity.WarningMinutes, entity.ActiveFrom, entity.ActiveTo, entity.IsActive, true);

        mapper.Setup(m => m.Map<SlaEntity>(request)).Returns(entity);
        mapper.Setup(m => m.Map<SlaResponse>(entity)).Returns(response);

        var handler = new AddSlaCommandHandler(repo.Object, audit.Object, currentUser.Object, mapper.Object);
        var result = await handler.Handle(new AddSlaCommand(request), CancellationToken.None);

        Assert.NotNull(result);
        repo.Verify(r => r.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        audit.Verify(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateSla_WhenNotFound_ReturnsNull()
    {
        var repo = new Mock<ISlaRepository>();
        var caseRepo = new Mock<ICaseRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        repo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((SlaEntity?)null);

        var handler = new UpdateSlaCommandHandler(repo.Object, caseRepo.Object, audit.Object,
            currentUser.Object, mapper.Object);

        var request = new UpdateSlaRequest(99, 1, "Updated", 60, 30, null, null, true);
        var result = await handler.Handle(new UpdateSlaCommand(99, request), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateSla_WhenValid_UpdatesAndAudits()
    {
        var repo = new Mock<ISlaRepository>();
        var caseRepo = new Mock<ICaseRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        currentUser.SetupGet(x => x.UserId).Returns(1);
        currentUser.SetupGet(x => x.CorrelationId).Returns("corr-1");
        audit.Setup(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var entity = new SlaEntity
        {
            Id = 8,
            TenantId = 1,
            Name = "Standard",
            TargetMinutes = 60,
            WarningMinutes = 30,
            IsActive = true
        };

        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        caseRepo.Setup(r => r.CountOpenBySlaIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(0);

        var request = new UpdateSlaRequest(entity.Id, 1, "Updated", 90, 30, null, null, true);
        mapper.Setup(m => m.Map(request, entity)).Callback(() =>
        {
            entity.Name = request.Name;
            entity.TargetMinutes = request.TargetMinutes;
            entity.WarningMinutes = request.WarningMinutes;
            entity.ActiveFrom = request.ActiveFrom;
            entity.ActiveTo = request.ActiveTo;
            entity.IsActive = request.IsActive;
        });
        mapper.Setup(m => m.Map<SlaResponse>(entity)).Returns(
            new SlaResponse(entity.Id, entity.TenantId, entity.Name, entity.TargetMinutes,
                entity.WarningMinutes, entity.ActiveFrom, entity.ActiveTo, entity.IsActive, true));

        var handler = new UpdateSlaCommandHandler(repo.Object, caseRepo.Object, audit.Object,
            currentUser.Object, mapper.Object);

        var result = await handler.Handle(new UpdateSlaCommand(entity.Id, request), CancellationToken.None);

        Assert.NotNull(result);
        repo.Verify(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
        repo.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        audit.Verify(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ToggleSla_WhenValid_ReturnsDto()
    {
        var repo = new Mock<ISlaRepository>();
        var caseRepo = new Mock<ICaseRepository>();
        var audit = new Mock<IAuditService>();
        var mapper = new Mock<IMapper>();
        var currentUser = new Mock<ICurrentUserContext>();

        var entity = new SlaEntity
        {
            Id = 3,
            TenantId = 1,
            Name = "Standard",
            TargetMinutes = 60,
            WarningMinutes = 30,
            IsActive = true
        };

        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        caseRepo.Setup(r => r.CountOpenBySlaIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(0);

        audit.Setup(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var response = new SlaResponse(entity.Id, entity.TenantId, entity.Name, entity.TargetMinutes,
            entity.WarningMinutes, entity.ActiveFrom, entity.ActiveTo, false, true);
        mapper.Setup(m => m.Map<SlaResponse>(entity)).Returns(response);

        var handler = new ToggleSlaCommandHandler(repo.Object, caseRepo.Object, audit.Object,
            currentUser.Object, mapper.Object);

        var result = await handler.Handle(new ToggleSlaCommand(entity.Id, new ToggleSlaRequest(1, false)),
            CancellationToken.None);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetSlaById_WhenExists_ReturnsDto()
    {
        var repo = new Mock<ISlaRepository>();
        var mapper = new Mock<IMapper>();
        var entity = new SlaEntity { Id = 7, TenantId = 1, Name = "SLA", TargetMinutes = 30 };
        var response = new SlaResponse(entity.Id, entity.TenantId, entity.Name, entity.TargetMinutes,
            entity.WarningMinutes, entity.ActiveFrom, entity.ActiveTo, entity.IsActive, true);

        repo.Setup(r => r.GetByIdAsync(entity.Id, It.IsAny<CancellationToken>())).ReturnsAsync(entity);
        mapper.Setup(m => m.Map<SlaResponse>(entity)).Returns(response);

        var handler = new GetSlaByIdQueryHandler(repo.Object, mapper.Object);
        var result = await handler.Handle(new GetSlaByIdQuery(entity.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(entity.Id, result!.Id);
    }

    [Fact]
    public async Task GetAllSlas_ReturnsPagedResult()
    {
        var repo = new Mock<ISlaRepository>();
        var mapper = new Mock<IMapper>();
        var entities = new List<SlaEntity> { new() { Id = 1, TenantId = 1, Name = "SLA", TargetMinutes = 30 } };
        var responses = new List<SlaResponse>
        {
            new(1, 1, "SLA", 30, null, null, null, false, true)
        };

        repo.Setup(r => r.GetAllPagedAsync(1, 10, It.IsAny<CancellationToken>())).ReturnsAsync(entities);
        repo.Setup(r => r.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        mapper.Setup(m => m.Map<IReadOnlyCollection<SlaResponse>>(entities)).Returns(responses);

        var handler = new GetAllSlasQueryHandler(repo.Object, mapper.Object);
        var result = await handler.Handle(new GetAllSlasQuery(1, 10), CancellationToken.None);

        Assert.Equal(1, result.Total);
        Assert.Single(result.Items);
    }

    [Fact]
    public async Task CheckCaseSla_WhenCaseMissing_ReturnsNull()
    {
        var caseRepo = new Mock<ICaseRepository>();
        var slaRepo = new Mock<ISlaRepository>();

        caseRepo.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((CaseEntity?)null);

        var handler = new CheckCaseSlaQueryHandler(caseRepo.Object, slaRepo.Object);
        var request = new CheckCaseSlaRequest(1, 99, DateTime.UtcNow);

        var result = await handler.Handle(new CheckCaseSlaQuery(request), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task CheckCaseSla_WhenValid_ReturnsStatus()
    {
        var caseRepo = new Mock<ICaseRepository>();
        var slaRepo = new Mock<ISlaRepository>();

        var now = new DateTime(2026, 2, 11, 12, 0, 0, DateTimeKind.Utc);
        var caseEntity = new CaseEntity
        {
            Id = 10,
            TenantId = 1,
            Status = CaseEntity.StatusOpen,
            Priority = CaseEntity.PriorityLow,
            SlaId = 5,
            CreatedAt = now.AddMinutes(-20),
            DueAt = now.AddMinutes(10)
        };

        var sla = new SlaEntity
        {
            Id = 5,
            TenantId = 1,
            Name = "Standard",
            TargetMinutes = 60,
            WarningMinutes = 30,
            IsActive = true
        };

        caseRepo.Setup(r => r.GetByIdAsync(caseEntity.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(caseEntity);
        slaRepo.Setup(r => r.GetByIdAsync(caseEntity.SlaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sla);

        var handler = new CheckCaseSlaQueryHandler(caseRepo.Object, slaRepo.Object);
        var request = new CheckCaseSlaRequest(1, caseEntity.Id, now);

        var result = await handler.Handle(new CheckCaseSlaQuery(request), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(caseEntity.Id, result!.CaseId);
        Assert.Equal(sla.Id, result.SlaId);
        Assert.False(result.IsBreached);
        Assert.True(result.IsActive);
    }
}
