# Observability & Resilience Backlog

Mini TOC:
1. [Scope and goal](#scope-and-goal)
2. [MVP](#mvp)
3. [Enterprise](#enterprise)
4. [Unit test suites](#unit-test-suites)
5. [Runbooks](#runbooks)
6. [Definition of done](#definition-of-done)

## Scope and goal

This backlog defines the implementation work for observability and resilience across all backend layers:
- API
- Application
- Contracts (requests and responses)
- Domain
- Infrastructure

It also defines test coverage for:
- `tests/EBOS.CRM.ApiTests`
- `tests/EBOS.CRM.ConcurrencyTests`
- `tests/EBOS.CRM.IntegrationTests`
- `tests/EBOS.CRM.StressTests`

## MVP

### API

- Add request correlation middleware:
  - Read `X-Correlation-Id` header.
  - Generate one when missing.
  - Include correlation id in response headers.
- Add global exception handling middleware:
  - Normalize unhandled exceptions into `ProblemDetails`.
  - Map transient dependency failures to retriable status codes (`503`/`504`).
- Add resilient endpoint behavior:
  - Enforce request timeout limits per endpoint group.
  - Return deterministic error payload for timeout/cancellation.
- Add baseline telemetry:
  - Structured logs for request start/end with latency and outcome.
  - Basic metrics counters/histograms for request count, failures, duration.

### Application

- Add MediatR pipeline behaviors:
  - Logging behavior with correlation id and handler name.
  - Validation behavior instrumentation with clear failure events.
  - Retry behavior for transient infrastructure exceptions (bounded retries + jitter).
- Introduce resilience policies per use case category:
  - Idempotent queries: retry + timeout.
  - Commands with side effects: timeout + circuit-breaker-aware guard (no unsafe retries).
- Add cancellation token propagation checks in all handlers.

### Contracts (Requests/Responses)

- Standardize error response contract:
  - `code`, `message`, `correlationId`, `details[]`, `retryable`.
- Add optional response metadata contract:
  - `traceId`, `elapsedMs`, `timestampUtc`.
- Version request contracts to include:
  - Idempotency key for write operations where applicable.
  - Optional client timeout hints for long-running operations.

### Domain

- Add domain error taxonomy:
  - `DomainValidation`, `DomainConflict`, `DomainRuleViolation`, `TransientDomainFailure`.
- Ensure aggregates expose deterministic failure reasons (no generic domain exceptions).
- Add idempotency-safe domain command semantics for critical write paths.

### Infrastructure

- Implement resilient data access baseline:
  - Database command timeout configuration.
  - Retry policy for transient DB/network errors.
  - Connection pool health logging and saturation warnings.
- Add outbound dependency protection:
  - HttpClient policies (timeout + retry + circuit breaker).
  - Structured logs for external dependency latency and failure class.
- Add health checks:
  - Liveness and readiness endpoints.
  - Readiness must include DB connectivity and critical dependencies.

## Enterprise

### API

- Add adaptive rate limiting and overload protection:
  - Tenant/client-aware quotas.
  - Graceful degradation responses with retry guidance.
- Add advanced observability headers:
  - W3C trace context propagation (`traceparent`, `tracestate`).
- Add endpoint SLO annotations and runtime breach logging.

### Application

- Add workflow-level resiliency orchestration:
  - Saga/compensation hooks for partial failure recovery.
  - Hedging for selected read-heavy low-risk operations.
- Add policy registry by operation criticality:
  - Platinum/Gold/Silver reliability profiles.
- Add dynamic policy configuration reload without restart.

### Contracts (Requests/Responses)

- Extend contracts with resilience hints:
  - `retryAfterMs`, `throttleScope`, `degradationMode`.
- Add async operation contracts:
  - Standard operation status response (`pending`, `running`, `failed`, `completed`).
  - Polling and callback correlation fields.
- Add compatibility strategy for multi-version clients with observability fields.

### Domain

- Add explicit domain-level compensating actions for reversible operations.
- Introduce reliability-related invariants:
  - Prevent duplicate business actions under retries.
  - Enforce monotonic state transitions in long-running workflows.
- Add domain event classification for operational analytics:
  - Business event vs technical event vs anomaly event.

### Infrastructure

- Implement telemetry platform integration:
  - OpenTelemetry tracing, metrics, and logs export.
  - Unified resource attributes (service/version/environment/tenant scope).
- Add durable messaging resilience:
  - Outbox/inbox patterns with deduplication.
  - Dead-letter handling and poison message diagnostics.
- Add persistence hardening:
  - Read replicas/failover routing strategy.
  - Bulkhead isolation for critical repositories.
- Add observability operations:
  - Dashboard definitions for SLI/SLO.
  - Alerting rules and escalation policy mapping.

## Unit test suites

### ApiTests

- Correlation id middleware:
  - Uses incoming id when present.
  - Generates id when absent.
  - Returns correlation id in response.
- Exception mapping middleware:
  - Maps domain errors to expected status and contract.
  - Maps transient dependency errors to retryable responses.
- Timeout/cancellation behavior:
  - Returns deterministic timeout payload.
  - Preserves correlation id in failure responses.
- Health endpoint behavior:
  - Liveness independent from dependencies.
  - Readiness fails when critical dependency is unavailable.

### ConcurrencyTests

- Idempotency under concurrent identical commands.
- Retry + timeout race conditions do not duplicate writes.
- Circuit breaker transitions under concurrent failure bursts.
- Contention scenarios validate invariant preservation and no inconsistent states.

### IntegrationTests

- End-to-end trace propagation across API -> Application -> Infrastructure.
- Resilience policy behavior with real infrastructure doubles:
  - transient DB failure then success.
  - dependency timeout then fallback/degraded path.
- Contract verification:
  - Error payload shape (`code/message/correlationId/retryable`).
  - Response metadata consistency (`traceId/elapsedMs/timestampUtc`).
- Health checks and readiness behavior with dependency toggling.

### StressTests

- Sustained load validates latency percentile targets and error budget behavior.
- Burst load validates rate-limiting and graceful degradation.
- Dependency brownout simulation validates circuit breaker effectiveness.
- Long-run soak test validates no telemetry/log pipeline bottleneck or memory growth regressions.

## Runbooks

- Main runbook (English):
  - `documentation/RunBooks/Observability-Operability-RunBook.md`
- Main runbook (Spanish):
  - `documentation/RunBooks/Observability-Operability-RunBook_ES.md`
- Post-deploy checklist (English):
  - `documentation/RunBooks/Observability-PostDeploy-Checklist.md`
- Post-deploy checklist (Spanish):
  - `documentation/RunBooks/Observability-PostDeploy-Checklist_ES.md`
- Drill record template (English):
  - `documentation/RunBooks/Observability-Drill-Record-Template.md`
- Drill record template (Spanish):
  - `documentation/RunBooks/Observability-Drill-Record-Template_ES.md`

Runbook scope to keep updated with implementation:
- Incident triage flow using `correlationId` and `traceId` from failing requests.
- Operational endpoints (`/health/live`, `/health/ready`, and observability readiness endpoint when enabled).
- Resilience and observability alert rules aligned with production thresholds.
- Recovery procedures (migration validation, rollback, and post-incident evidence).
- Drill cadence and minimum evidence requirements for traceability and auditability.

Backlog deliverables related to runbooks:
- MVP:
  - Implement triage-ready error contracts returning `correlationId` and `traceId`.
  - Ensure logs and traces can be queried by those identifiers.
  - Validate runbook steps with at least one incident simulation in non-production.
- Enterprise:
  - Keep runbook aligned with adaptive rate limiting, advanced trace propagation, and telemetry platform integration.
  - Add SLO-driven alert tuning guidance and escalation path refinements.
  - Record drill outcomes and improve procedures based on measured RTO/RPO.

## Definition of done

- All listed layer tasks implemented for MVP.
- Enterprise backlog items are broken into sprint-ready stories with owner and estimate.
- Test cases added to all four suites with stable CI execution.
- Operational dashboards and alerts available for MVP critical paths.
- Runbooks created and maintained in EN/ES with incident triage steps using correlation id and trace id.
