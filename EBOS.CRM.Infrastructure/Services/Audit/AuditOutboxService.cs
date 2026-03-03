using System.Net.Http.Json;
using System.Diagnostics;
using System.Text.Json;
using EBOS.CRM.Contracts.Requests.Services;
using EBOS.CRM.Domain.Entities.EBOS;
using EBOS.CRM.Domain.Interfaces.Services;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using EBOS.CRM.Infrastructure.Observability;
using Microsoft.Extensions.Logging;

namespace EBOS.CRM.Infrastructure.Services.Audit;

public sealed class AuditOutboxService(
    CrmDbContext dbContext,
    IHttpClientFactory httpClientFactory,
    IOptions<AuditOutboxOptions> outboxOptions,
    ILogger<AuditOutboxService> logger,
    ICustomer360Metrics metrics) : IAuditOutboxService
{
    private static readonly ActivitySource ActivitySource = new(TelemetryNames.AuditActivitySource);
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task EnqueueAsync(string operation, AuditInsertRequest request, string? error,
        CancellationToken cancellationToken = default)
    {
        using var activity = ActivitySource.StartActivity("audit.outbox.enqueue", ActivityKind.Internal);
        activity?.SetTag("audit.operation", operation);
        activity?.SetTag("audit.correlation_id", request.CorrelationId);
        activity?.SetTag("audit.has_error", !string.IsNullOrWhiteSpace(error));
        activity?.SetTag("user.id", request.UserId);

        if (!outboxOptions.Value.Enabled)
        {
            activity?.SetTag("audit.outbox.enabled", false);
            return;
        }

        activity?.SetTag("audit.outbox.enabled", true);

        var now = DateTime.UtcNow;
        dbContext.AuditOutboxMessages.Add(new AuditOutboxMessage
        {
            Operation = operation,
            Payload = JsonSerializer.Serialize(request, JsonOptions),
            CreatedAt = now,
            NextAttemptAt = now,
            AttemptCount = 0,
            LastError = error
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        metrics.RecordAuditOutboxEnqueue(operation);
    }

    public async Task<int> DispatchPendingAsync(CancellationToken cancellationToken = default)
    {
        if (!outboxOptions.Value.Enabled)
        {
            return 0;
        }

        var now = DateTime.UtcNow;
        var batch = await dbContext.AuditOutboxMessages
            .Where(x => x.ProcessedAt == null && x.NextAttemptAt <= now)
            .OrderBy(x => x.NextAttemptAt)
            .Take(Math.Max(1, outboxOptions.Value.BatchSize))
            .ToListAsync(cancellationToken);

        if (batch.Count == 0)
        {
            return 0;
        }

        var sent = 0;
        var client = httpClientFactory.CreateClient(nameof(AuditServiceClient));
        var endpoint = "api/audit/InsertAudit";

        foreach (var message in batch)
        {
            using var activity = ActivitySource.StartActivity("audit.outbox.dispatch", ActivityKind.Producer);
            activity?.SetTag("audit.outbox.message_id", message.Id);
            activity?.SetTag("audit.operation", message.Operation);
            activity?.SetTag("audit.outbox.attempt_count", message.AttemptCount);

            try
            {
                var request = JsonSerializer.Deserialize<AuditInsertRequest>(message.Payload, JsonOptions);
                if (request is null)
                {
                    throw new InvalidOperationException("Invalid outbox payload.");
                }

                activity?.SetTag("audit.correlation_id", request.CorrelationId);
                activity?.SetTag("user.id", request.UserId);
                client.DefaultRequestHeaders.Remove("X-Correlation-Id");
                client.DefaultRequestHeaders.Add("X-Correlation-Id", request.CorrelationId);

                var response = await client.PostAsJsonAsync(endpoint, request, JsonOptions, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    message.ProcessedAt = DateTime.UtcNow;
                    sent++;
                    activity?.SetStatus(ActivityStatusCode.Ok);
                    metrics.RecordAuditOutboxDispatch(message.Operation, true);
                    continue;
                }

                var error = await response.Content.ReadAsStringAsync(cancellationToken);
                RegisterFailure(message, $"HTTP {(int)response.StatusCode}: {error}");
                activity?.SetStatus(ActivityStatusCode.Error, $"HTTP {(int)response.StatusCode}");
                metrics.RecordAuditOutboxDispatch(message.Operation, false);
                logger.LogWarning(
                    "Audit outbox dispatch failed for message {MessageId} operation {Operation}. Status={StatusCode}. TraceId={TraceId}",
                    message.Id,
                    message.Operation,
                    (int)response.StatusCode,
                    activity?.TraceId.ToString() ?? Activity.Current?.TraceId.ToString());
            }
            catch (Exception ex)
            {
                RegisterFailure(message, ex.Message);
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                metrics.RecordAuditOutboxDispatch(message.Operation, false);
                logger.LogWarning(ex,
                    "Audit outbox dispatch threw for message {MessageId} operation {Operation}. TraceId={TraceId}",
                    message.Id,
                    message.Operation,
                    activity?.TraceId.ToString() ?? Activity.Current?.TraceId.ToString());
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return sent;
    }

    private void RegisterFailure(AuditOutboxMessage message, string error)
    {
        message.AttemptCount += 1;
        message.LastError = error.Length <= 2000 ? error : error[..2000];

        var maxAttempts = Math.Max(1, outboxOptions.Value.MaxAttempts);
        if (message.AttemptCount >= maxAttempts)
        {
            message.ProcessedAt = DateTime.UtcNow;
            return;
        }

        var delaySeconds = Math.Min(300, (int)Math.Pow(2, message.AttemptCount));
        message.NextAttemptAt = DateTime.UtcNow.AddSeconds(delaySeconds);
    }
}
