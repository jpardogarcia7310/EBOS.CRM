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
- Definition and operational criteria:
  - API observability is the first triage layer: every failed request must be diagnosable with `correlationId` + `traceId`.
  - Error mapping must be deterministic and stable across releases for the same failure class.
  - Timeout and cancellation responses must use a single payload shape and include retry guidance.
- Unit-test expectations:
  - Verify status mapping for each domain error taxonomy class.
  - Verify timeout payload schema and header propagation.
  - Verify logs include operation name, latency bucket, status class, and identifiers.
- Runbook references for API:
  - Triage flow starts at API request log -> correlation search -> trace timeline.
  - Include a decision branch for `4xx non-retryable` vs `5xx retryable/transient`.

### Application

- Add MediatR pipeline behaviors:
  - Logging behavior with correlation id and handler name.
  - Validation behavior instrumentation with clear failure events.
  - Retry behavior for transient infrastructure exceptions (bounded retries + jitter).
- Introduce resilience policies per use case category:
  - Idempotent queries: retry + timeout.
  - Commands with side effects: timeout + circuit-breaker-aware guard (no unsafe retries).
- Add cancellation token propagation checks in all handlers.
- Definition and operational criteria:
  - Application layer owns resilience orchestration boundaries; retries are policy-driven, not ad hoc in handlers.
  - Commands with side effects are non-retriable by default unless idempotency is explicitly guaranteed.
  - Pipeline behaviors must emit structured events for validation failures, retries, and final outcomes.
- Unit-test expectations:
  - Validate policy selection per handler type (query vs command).
  - Validate bounded retry count and jitter window for transient failures.
  - Validate cancellation token is honored before expensive I/O.
- Runbook references for Application:
  - Handler-level failure triage: inspect behavior events before infrastructure logs.
  - Retry storm check: detect repeated attempts for the same operation key.

### Contracts (Requests/Responses)

- Standardize error response contract:
  - `code`, `message`, `correlationId`, `details[]`, `retryable`.
- Add optional response metadata contract:
  - `traceId`, `elapsedMs`, `timestampUtc`.
- Version request contracts to include:
  - Idempotency key for write operations where applicable.
  - Optional client timeout hints for long-running operations.
- Definition and operational criteria:
  - Contracts define the support boundary with clients; observability fields are part of the compatibility commitment.
  - `retryable` is computed by server policy, not copied from client intent.
  - `details[]` must be machine-readable and bounded to prevent payload abuse.
- Unit-test expectations:
  - Validate schema conformance for success/error payloads.
  - Validate backward compatibility for optional observability fields.
  - Validate deterministic error code generation by taxonomy.
- Runbook references for Contracts:
  - Contract drift checks for deployed versions.
  - Client troubleshooting guide based on `code`, `retryable`, and `details[]`.

### Domain

- Add domain error taxonomy:
  - `DomainValidation`, `DomainConflict`, `DomainRuleViolation`, `TransientDomainFailure`.
- Clarify taxonomy concept and usage:
  - In this backlog, a "taxonomy" is a classification model for domain failures, not domain entities.
  - These types are error categories (usually represented as exception types or error codes) used to standardize behavior, logging, and API mapping.
  - Purpose of the taxonomy: produce deterministic failure handling, avoid generic exceptions, improve observability, and enable consistent retries/fallbacks.
- Define each domain error taxonomy type:
  - `DomainValidation`: Input/state shape is invalid before applying business logic (missing required value, invalid format, out-of-range value). Usually mapped to non-retriable client correction.
  - `DomainConflict`: Requested operation collides with current persisted/domain state (duplicate key, version mismatch, already-processed command). May be retriable only when conflict is concurrency-related and caller can safely retry.
  - `DomainRuleViolation`: A strict business invariant is broken even with syntactically valid input (credit limit exceeded, illegal state transition). Non-retriable until business conditions change.
  - `TransientDomainFailure`: Temporary domain-level execution barrier caused by short-lived conditions (domain service temporarily unavailable, lock timeout, transient stale read). Retriable with bounded backoff/jitter.
- Ensure aggregates expose deterministic failure reasons (no generic domain exceptions).
- Add idempotency-safe domain command semantics for critical write paths.
- Unit-test expectations:
  - One test set per taxonomy type with deterministic classification assertions.
  - Concurrency tests for `DomainConflict` (version clash/replayed command).
  - Invariant tests for `DomainRuleViolation` with explicit business preconditions.
- Runbook references for Domain:
  - Classification decision tree: validation vs conflict vs rule violation vs transient.
  - Recovery action matrix: client fix, safe retry, or business remediation.

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
- Definition and operational criteria:
  - Infrastructure observability provides dependency health, latency, and saturation evidence for incident scope.
  - Transient classification criteria must be explicit per dependency type (DB, network, HTTP, broker).
  - Readiness failure must include dependency identity and failure class.
- Unit-test expectations:
  - Validate timeout/retry/circuit-breaker policy composition per dependency.
  - Validate health-check behavior under dependency toggling.
  - Validate saturation warnings for connection pool thresholds.
- Runbook references for Infrastructure:
  - Dependency outage triage by failure class and blast radius.
  - Recovery checklist: failover, rollback, and post-recovery verification.

## Enterprise

### API

- Add adaptive rate limiting and overload protection:
  - Tenant/client-aware quotas.
  - Graceful degradation responses with retry guidance.
- Add advanced observability headers:
  - W3C trace context propagation (`traceparent`, `tracestate`).
- Add endpoint SLO annotations and runtime breach logging.
- Definition and operational criteria:
  - Enterprise API extends MVP with adaptive controls and SLO governance.
  - Rate limits must be observable by tenant/client dimensions and linked to degradation mode.
- Unit-test expectations:
  - Verify tenant-scoped throttling and consistent retry-after semantics.
  - Verify trace context propagation across async boundaries.
- Runbook references for API:
  - Overload management procedure with throttle tuning steps and escalation criteria.

### Application

- Add workflow-level resiliency orchestration:
  - Saga/compensation hooks for partial failure recovery.
  - Hedging for selected read-heavy low-risk operations.
- Add policy registry by operation criticality:
  - Platinum/Gold/Silver reliability profiles.
- Add dynamic policy configuration reload without restart.
- Definition and operational criteria:
  - Enterprise workflows require compensating logic and policy profiles by operation criticality.
  - Dynamic policy reload must be auditable and reversible.
- Unit-test expectations:
  - Verify compensation execution order and idempotent recovery.
  - Verify hot-reload policy changes do not break in-flight operations.
- Runbook references for Application:
  - Partial-failure workflow recovery procedure.
  - Policy rollback playbook for unstable runtime behavior.

### Contracts (Requests/Responses)

- Extend contracts with resilience hints:
  - `retryAfterMs`, `throttleScope`, `degradationMode`.
- Add async operation contracts:
  - Standard operation status response (`pending`, `running`, `failed`, `completed`).
  - Polling and callback correlation fields.
- Add compatibility strategy for multi-version clients with observability fields.
- Definition and operational criteria:
  - Enterprise contracts formalize async lifecycle, throttling hints, and degradation transparency.
  - Multi-version support must include deprecation windows and migration guidance.
- Unit-test expectations:
  - Verify async state transition contracts and correlation fields.
  - Verify backward compatibility matrix for versioned clients.
- Runbook references for Contracts:
  - Client version incident triage and compatibility verification steps.

### Domain

- Add explicit domain-level compensating actions for reversible operations.
- Introduce reliability-related invariants:
  - Prevent duplicate business actions under retries.
  - Enforce monotonic state transitions in long-running workflows.
- Add domain event classification for operational analytics:
  - Business event vs technical event vs anomaly event.
- Definition and operational criteria:
  - Enterprise domain extends MVP taxonomy with compensations, reliability invariants, and event governance.
  - Duplicate-business-action prevention is mandatory under retries and distributed retries.
- Unit-test expectations:
  - Verify compensating actions preserve invariants after partial failure.
  - Verify monotonic transitions in long-running workflows.
  - Verify event classification consistency for analytics consumers.
- Runbook references for Domain:
  - Business remediation path for non-retriable rule violations.
  - Compensation replay and audit evidence procedure.

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
- Definition and operational criteria:
  - Enterprise infrastructure must provide end-to-end telemetry with operationally useful dimensions.
  - Messaging resilience requires deduplication evidence and dead-letter diagnosability.
- Unit-test expectations:
  - Verify outbox/inbox deduplication correctness under concurrent delivery.
  - Verify dead-letter routing and poison message diagnostics.
  - Verify SLI/SLO dashboard signal correctness from emitted telemetry.
- Runbook references for Infrastructure:
  - Telemetry pipeline degradation procedure.
  - Dead-letter backlog drain and replay procedure with risk controls.

## Unit test suites

Layer coverage policy (MVP and Enterprise):
- API: deterministic error mapping, header propagation, timeout contracts, health endpoint behavior.
- Application: policy selection, retries/timeouts/cancellation, compensation and hot-reload behavior.
- Contracts: schema/version compatibility, error code determinism, async lifecycle contracts.
- Domain: taxonomy classification, invariant enforcement, conflict/idempotency behavior.
- Infrastructure: dependency failure simulation, policy composition, saturation and messaging resilience.

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
  - `documentation/RunBooks/Observability/Observability-Operability-RunBook.md`
- Main runbook (Spanish):
  - `documentation/RunBooks/Observability/Observability-Operability-RunBook_ES.md`
- Post-deploy checklist (English):
  - `documentation/RunBooks/Observability/Observability-PostDeploy-Checklist.md`
- Post-deploy checklist (Spanish):
  - `documentation/RunBooks/Observability/Observability-PostDeploy-Checklist_ES.md`
- Drill record template (English):
  - `documentation/RunBooks/Drills/Observability/Observability-Drill-Execution-Template.md`
- Drill record template (Spanish):
  - `documentation/RunBooks/Drills/Observability/Observability-Drill-Execution-Template_ES.md`

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

Runbook minimum content by layer (applies to MVP and Enterprise):
- API: request-level triage steps, error-contract interpretation, throttle/degradation response.
- Application: handler/pipeline inspection sequence, retry storm containment, compensation validation.
- Contracts: payload validation checklist, compatibility/version triage, client communication template.
- Domain: taxonomy decision tree, non-retriable business remediation, idempotency conflict resolution.
- Infrastructure: dependency diagnosis matrix, failover/recovery checklist, telemetry pipeline health checks.

## Definition of done

- All listed layer tasks implemented for MVP.
- Enterprise backlog items are broken into sprint-ready stories with owner and estimate.
- Test cases added to all four suites with stable CI execution.
- Operational dashboards and alerts available for MVP critical paths.
- Runbooks created and maintained in EN/ES with incident triage steps using correlation id and trace id.

## Appendix A - Executive Version

Purpose:
- Provide a concise implementation and governance view for leadership and planning.

MVP by layer:
- API: deterministic error mapping, request correlation, timeout handling, and baseline logs/metrics.
- Application: MediatR resilience policies by operation type, bounded retries for transient failures, and cancellation propagation.
- Contracts: standardized error envelope (`code/message/correlationId/details/retryable`) and observability metadata.
- Domain: explicit domain error taxonomy with deterministic failure classification and idempotent command semantics on critical writes.
- Infrastructure: resilient data/dependency access, health checks, and saturation visibility.

Enterprise by layer:
- API: adaptive rate limiting, advanced trace propagation, endpoint-level SLO tracking.
- Application: compensation workflows, criticality profiles, dynamic policy reload.
- Contracts: async lifecycle contracts, throttling/degradation hints, multi-version compatibility.
- Domain: reliability invariants, compensating actions, operational event classification.
- Infrastructure: OpenTelemetry platform integration, durable messaging resilience, SLI/SLO dashboards and alerting.

Executive checkpoints:
- Delivery: all MVP controls in production paths and Enterprise items decomposed with owner/estimate.
- Operability: runbooks validated through drills and incident simulations.
- Quality: stable CI with unit/integration/concurrency/stress coverage for resilience controls.

## Appendix B - Audit Version

Purpose:
- Define auditable controls, expected evidence, and pass/fail criteria for observability and resilience.

Control model:
- Control ID format: `OBS-{MVP|ENT}-{LAYER}-{NN}`.
- Evidence sources: tests, CI logs, application logs, traces, dashboards, runbook drill records.
- Result states: `Pass`, `Partial`, `Fail`, with mandatory remediation action for non-pass results.

MVP controls by layer:
- API:
  - Control: domain taxonomy errors map to documented HTTP statuses and payload contract.
  - Evidence: API unit tests + sample production log/trace for each error class.
  - Pass criteria: deterministic mapping, identifiers present, retryability flag consistent.
- Application:
  - Control: retries/timeouts/cancellation follow policy by handler category.
  - Evidence: unit tests for behavior pipelines + retry telemetry.
  - Pass criteria: bounded retries, no unsafe command retries, cancellation honored.
- Contracts:
  - Control: error/metadata schema stability and compatibility.
  - Evidence: contract tests and version compatibility checks.
  - Pass criteria: no schema regressions for supported clients.
- Domain:
  - Control: deterministic classification into `DomainValidation`, `DomainConflict`, `DomainRuleViolation`, `TransientDomainFailure`.
  - Evidence: taxonomy-specific unit tests and concurrency tests for conflict/idempotency.
  - Pass criteria: no generic domain exceptions in critical paths.
- Infrastructure:
  - Control: dependency resilience and health-readiness correctness.
  - Evidence: integration tests with dependency fault injection + health endpoint checks.
  - Pass criteria: transient faults retried per policy, readiness fails with dependency context.

Enterprise controls by layer:
- API:
  - Control: adaptive throttling and SLO breach observability.
  - Evidence: stress tests, throttle telemetry, alert records.
  - Pass criteria: policy-aligned throttling with actionable alerting.
- Application:
  - Control: compensation logic and dynamic policy reload safety.
  - Evidence: workflow tests for partial failures + policy reload audit logs.
  - Pass criteria: recoverable workflows, reversible policy changes.
- Contracts:
  - Control: async contract lifecycle and multi-version compatibility.
  - Evidence: compatibility matrix + async contract tests.
  - Pass criteria: versioned clients operate within published compatibility window.
- Domain:
  - Control: reliability invariants and event classification governance.
  - Evidence: invariant tests + event catalog validation.
  - Pass criteria: monotonic transitions preserved and duplicate business actions prevented.
- Infrastructure:
  - Control: messaging resilience, dead-letter handling, telemetry platform integrity.
  - Evidence: outbox/inbox tests, dead-letter drill evidence, telemetry pipeline health checks.
  - Pass criteria: deduplication correctness and recoverable dead-letter processing.

Audit cadence and runbook obligations:
- MVP: monthly control review and at least one non-production incident drill per quarter.
- Enterprise: monthly control review, quarterly resilience game day, and SLO alert tuning review.
- Mandatory runbook evidence: triage timeline, decision path, remediation action, verification of recovery, and follow-up backlog item.
