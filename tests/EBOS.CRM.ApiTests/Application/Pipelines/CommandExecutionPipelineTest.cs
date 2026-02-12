using EBOS.Core.Primitives.Interfaces;
using EBOS.CRM.Application.Shared.Commands;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Contracts.Responses.Services;
using EBOS.CRM.Domain.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EBOS.CRM.ApiTests.Application.Pipelines;

public class CommandExecutionPipelineTest
{
    [Fact]
    public async Task ExecuteAsync_WhenConcurrencyRetryOccurs_UsesSingleAuditInsert()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var auditService = new Mock<IAuditService>();
        var pipeline = CreatePipeline(auditService.Object, retryCount: 2);

        var attempts = 0;
        var auditCalls = 0;

        auditService.Setup(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .Callback(() => auditCalls++)
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        var response = await pipeline.ExecuteAsync(
            unitOfWork.Object,
            _ =>
            {
                attempts++;
                if (attempts == 1)
                {
                    throw new DbUpdateConcurrencyException("collision");
                }

                return Task.FromResult(new CommandExecutionResult<string>(
                    Response: "ok",
                    BuildAuditRequest: () => new AuditInsertRequest(
                        UserId: 1,
                        TimeStamp: DateTimeOffset.UtcNow,
                        Action: "Add",
                        Entity: "Test",
                        RegisterId: 10,
                        OldValues: null,
                        NewValues: "{}",
                        CorrelationId: "corr-1")));
            },
            CancellationToken.None);

        Assert.Equal("ok", response);
        Assert.Equal(2, attempts);
        Assert.Equal(1, auditCalls);
    }

    [Fact]
    public async Task ExecuteAsync_WhenFirstAttemptCollides_RetriesOperation()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var auditService = new Mock<IAuditService>();
        var pipeline = CreatePipeline(auditService.Object, retryCount: 2);

        var attempts = 0;

        var response = await pipeline.ExecuteAsync(
            unitOfWork.Object,
            _ =>
            {
                attempts++;
                if (attempts == 1)
                {
                    throw new DbUpdateConcurrencyException("collision");
                }

                return Task.FromResult(new CommandExecutionResult<int>(
                    Response: 42,
                    BuildAuditRequest: null));
            },
            CancellationToken.None);

        Assert.Equal(42, response);
        Assert.Equal(2, attempts);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCollisionsExceedRetries_ThrowsConcurrencyException()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var auditService = new Mock<IAuditService>();
        var pipeline = CreatePipeline(auditService.Object, retryCount: 2);

        await Assert.ThrowsAsync<CommandConcurrencyException>(() =>
            pipeline.ExecuteAsync<string>(
                unitOfWork.Object,
                _ => throw new DbUpdateConcurrencyException("collision"),
                CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_CommitsBeforeAuditInsert()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var auditService = new Mock<IAuditService>();
        var pipeline = CreatePipeline(auditService.Object, retryCount: 1);

        var sequence = new MockSequence();
        unitOfWork.InSequence(sequence)
            .Setup(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        unitOfWork.InSequence(sequence)
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        auditService.InSequence(sequence)
            .Setup(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AuditInsertResponse(true, 1));

        await pipeline.ExecuteAsync(
            unitOfWork.Object,
            _ => Task.FromResult(new CommandExecutionResult<string>(
                Response: "ok",
                BuildAuditRequest: () => new AuditInsertRequest(
                    UserId: 1,
                    TimeStamp: DateTimeOffset.UtcNow,
                    Action: "Add",
                    Entity: "Test",
                    RegisterId: 10,
                    OldValues: null,
                    NewValues: "{}",
                    CorrelationId: "corr-1"))),
            CancellationToken.None);

        unitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        auditService.Verify(a => a.InsertAuditAsync(It.IsAny<AuditInsertRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private static CommandExecutionPipeline CreatePipeline(IAuditService auditService, int retryCount)
    {
        var options = Microsoft.Extensions.Options.Options.Create(new CommandExecutionOptions
        {
            ConcurrencyRetryCount = retryCount,
            ConcurrencyRetryDelayMs = 1
        });

        return new CommandExecutionPipeline(auditService, options);
    }
}
