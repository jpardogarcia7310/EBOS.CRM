using System.Text;
using EBOS.CRM.Domain.Identity;
using EBOS.CRM.Domain.Interfaces.Services.CRM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EBOS.CRM.Api.Controllers.Observability;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = PolicyKeys.Operations.ObservabilityRead)]
public sealed class MetricsController(ICustomer360Metrics metrics) : ControllerBase
{
    [HttpGet]
    [Route("metrics")]
    public IActionResult Get()
    {
        var snapshot = metrics.GetSnapshot();
        var mergeSuccess = Math.Max(0, snapshot.MergeTotal - snapshot.MergeFailures);
        var concurrencyRetried = Math.Max(0, snapshot.ConcurrencyConflictTotal - snapshot.ConcurrencyFailureTotal);
        var sb = new StringBuilder(1024);

        AppendCounter(sb, "customer360_merge_total", "success=\"true\"", mergeSuccess);
        AppendCounter(sb, "customer360_merge_total", "success=\"false\"", snapshot.MergeFailures);
        AppendCounter(sb, "customer360_dedupe_query_total", null, snapshot.DedupeQueryTotal);
        AppendCounter(sb, "customer360_consent_event_total", "granted=\"true\"", snapshot.ConsentGrantedTotal);
        AppendCounter(sb, "customer360_consent_event_total", "granted=\"false\"", snapshot.ConsentRevokedTotal);
        AppendCounter(sb, "customer360_audit_outbox_total", "event=\"enqueue\"", snapshot.AuditOutboxEnqueueTotal);
        AppendCounter(sb, "customer360_audit_outbox_total",
            "event=\"dispatch\",success=\"true\"",
            snapshot.AuditOutboxDispatchSuccessTotal);
        AppendCounter(sb, "customer360_audit_outbox_total",
            "event=\"dispatch\",success=\"false\"",
            snapshot.AuditOutboxDispatchFailureTotal);
        AppendCounter(sb, "customer360_concurrency_total",
            "exhausted_retries=\"false\"",
            concurrencyRetried);
        AppendCounter(sb, "customer360_concurrency_total",
            "exhausted_retries=\"true\"",
            snapshot.ConcurrencyFailureTotal);

        return Content(sb.ToString(), "text/plain; version=0.0.4; charset=utf-8");
    }

    private static void AppendCounter(StringBuilder sb, string metricName, string? labels, long value)
    {
        sb.Append("# TYPE ").Append(metricName).AppendLine(" counter");
        sb.Append(metricName);
        if (!string.IsNullOrWhiteSpace(labels))
        {
            sb.Append('{').Append(labels).Append('}');
        }

        sb.Append(' ').Append(value).AppendLine();
    }
}
