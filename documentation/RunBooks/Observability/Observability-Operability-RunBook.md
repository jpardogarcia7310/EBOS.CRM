# Observability and Resilience Operability Runbook

## Scope
- Observability and resilience operation for request handling, dependencies, data access, and background processing.
- Applies to `EBOS.CRM.Api`, `EBOS.CRM.Application`, and `EBOS.CRM.Infrastructure`.

## Service Objectives (SLO)
- RTO (service recovery):
  - P1 (API unavailable): <= 30 minutes
  - P2 (degraded resilience behavior): <= 2 hours
- RPO (acceptable data/telemetry loss):
  - transactional business data: <= 5 minutes
  - telemetry continuity (logs/metrics/traces): <= 15 minutes
- Escalation:
  - P1 -> immediate on-call paging, incident channel active, updates every 15 minutes.
  - P2 -> business-hours on-call response, updates every 60 minutes.

## Operational Endpoints
- Health checks:
  - Liveness: `GET /health/live`
  - Readiness: `GET /health/ready`
- Observability metadata endpoint (if enabled):
  - `GET /api/v2.0/OperationalReadiness/observability`

## Correlation and Tracing Standard
- Incoming correlation header: `X-Correlation-Id`.
- Trace context headers: `traceparent`, `tracestate`.
- Every error response must include:
  - `correlationId`
  - `traceId`
- Every incident note must record:
  - first failing `correlationId`
  - representative `traceId`
  - impacted endpoint and UTC timestamp.

## Metrics (Observability/Resilience)
- API request metrics:
  - `http.server.request.count`
  - `http.server.request.duration`
  - `http.server.request.failures`
- Dependency metrics:
  - `dependency.call.count`
  - `dependency.call.duration`
  - `dependency.call.failures`
- Resilience policy metrics:
  - `resilience.retry.total`
  - `resilience.circuitbreaker.open.total`
  - `resilience.timeout.total`
  - `resilience.ratelimit.reject.total`

## Recommended Dashboard Panels
- Traffic and latency:
  - requests per minute
  - p50/p95/p99 latency
- Reliability:
  - error rate by endpoint and status code class
  - retry rate and timeout rate
- Dependency health:
  - top failing dependencies by error class
  - circuit breaker open duration per dependency
- Saturation:
  - thread pool queue length
  - DB connection pool usage
  - rate-limited requests per minute

## Alerting Rules (baseline)
- API availability critical:
  - failed requests ratio >= 10% for 5 minutes.
- Latency critical:
  - p95 latency above objective threshold for 10 minutes.
- Circuit breaker critical:
  - breaker open state sustained > 5 minutes on critical dependency.
- Timeout critical:
  - timeout count above threshold for 5 minutes.
- Readiness degraded/unhealthy:
  - readiness endpoint is `Degraded` or `Unhealthy` for 3 consecutive probes.

## Configuration
- Section: `Observability`
  - `EnableCorrelationIdHeader`
  - `EnableTraceContextPropagation`
  - `SlowRequestThresholdMs`
- Section: `Resilience`
  - `RequestTimeoutMs`
  - `RetryMaxAttempts`
  - `RetryBaseDelayMs`
  - `CircuitBreakerFailureThreshold`
  - `CircuitBreakerSamplingWindowSeconds`
  - `CircuitBreakerBreakDurationSeconds`
  - `RateLimitPerMinute`

## Incident Triage Procedure (CorrelationId and TraceId)
1. Confirm alert type, scope, start time (UTC), and impacted endpoints.
2. Capture one failing request sample from logs or gateway and extract:
   - `correlationId`
   - `traceId`
   - endpoint, status code, and latency.
3. Query logs by `correlationId` to reconstruct the request lifecycle:
   - ingress log
   - handler logs
   - dependency calls
   - exception/failure log.
4. Query distributed traces by `traceId` to locate the failing span and identify bottleneck:
   - API span
   - Application handler span
   - Infrastructure/dependency span.
5. Classify incident cause:
   - dependency outage/latency
   - database saturation/deadlock/timeout
   - resilience policy misconfiguration
   - code regression.
6. Execute mitigation according to cause:
   - dependency issue: enable fallback/degradation path, reduce request concurrency.
   - DB issue: reduce write pressure, verify pool saturation, adjust timeout/retry as emergency control.
   - misconfiguration: rollback or hotfix policy values.
   - code regression: rollback deployment to previous stable version.
7. Validate recovery:
   - readiness back to healthy/degraded-acceptable
   - error rate and p95 within objective
   - circuit breaker and timeout alerts cleared.
8. Close incident with evidence:
   - correlationId/traceId samples
   - timeline (detection, mitigation, recovery)
   - root cause and prevention actions.

## Domain Classification and Recovery (MVP)

Classification decision tree (`DomainValidation` vs `DomainConflict` vs `DomainRuleViolation` vs `TransientDomainFailure`):
1. Is the input or aggregate state shape invalid before business invariants execute?
   - Yes -> classify as `DomainValidation`.
   - No -> continue.
2. Does the request collide with persisted/current state (version mismatch, duplicate/replayed command, competing writer)?
   - Yes -> classify as `DomainConflict`.
   - No -> continue.
3. Is a business invariant violated with otherwise valid input (illegal transition, append-only breach, forbidden business action)?
   - Yes -> classify as `DomainRuleViolation`.
   - No -> continue.
4. Is the failure caused by temporary, short-lived conditions at domain execution boundary (transient lock/availability/stale-read barrier)?
   - Yes -> classify as `TransientDomainFailure`.
   - No -> classify as unknown domain fault and escalate for taxonomy gap analysis.

Recovery action matrix:
- `DomainValidation`:
  - Primary action: client correction.
  - Retry policy: no automatic retry.
  - Operator action: confirm deterministic code/message and provide caller fix guidance.
- `DomainConflict`:
  - Primary action: safe retry only for concurrency/version conflicts.
  - Retry policy: bounded retry with jitter only when operation is idempotent.
  - Operator action: identify conflict subtype (`version_mismatch`, `command_replay`, `already_processed`) and verify idempotency key/command identity.
- `DomainRuleViolation`:
  - Primary action: business remediation.
  - Retry policy: do not retry until business preconditions change.
  - Operator action: route to business owner with invariant code and impacted entity id.
- `TransientDomainFailure`:
  - Primary action: safe retry.
  - Retry policy: bounded retry with backoff+jitter, then degrade/fail fast if threshold exceeded.
  - Operator action: validate dependency/transient indicators and clear alerts once stability is restored.

## Domain Enterprise Runbook References

### Business Remediation Path for Non-Retriable Rule Violations
1. Identify violation from deterministic domain code (`DOMAIN_RULE_VIOLATION_*`) and capture `correlationId` + `traceId`.
2. Confirm non-retriable classification:
   - taxonomy is `DomainRuleViolation`
   - business preconditions are still unsatisfied.
3. Open business remediation ticket with:
   - invariant code
   - impacted entity ids/tenant
   - first-seen and last-seen UTC timestamps
   - operational impact.
4. Apply approved business remediation action (data correction, state unlock, policy override, or business approval flow).
5. Re-run operation once remediation is complete and verify:
   - invariant no longer fails
   - no duplicate business side effects
   - expected domain event category remains stable.
6. Attach audit evidence:
   - ticket id and approver
   - before/after state evidence
   - trace/log samples tied to remediation.

### Compensation Replay and Audit Evidence Procedure
1. Select failed reversible workflow instances eligible for compensation replay.
2. Validate replay preconditions:
   - current status is replay-eligible (`FAILED` for privacy request flow)
   - compensating command is available and deterministic
   - idempotency guard is active.
3. Execute compensating command replay in controlled batches with correlation tracking.
4. Verify post-replay invariants:
   - state transitioned to expected compensated state
   - failure markers were cleared where required
   - monotonic transition rules remained valid.
5. Verify emitted operational events:
   - technical compensation event emitted
   - no category drift against event catalog.
6. Store audit evidence:
   - replay batch id and UTC execution window
   - list of affected entity ids
   - count of replayed/skipped/failed operations
   - sample `correlationId`/`traceId`
   - operator and approver.

## Migration Procedure
1. Backup DB and export current resilience/observability config.
2. Deploy API and infrastructure changes.
3. Validate startup:
   - `/health/live` is healthy
   - `/health/ready` is healthy or expected degraded.
4. Send controlled test requests and verify `correlationId` and `traceId` appear in responses and logs.
5. Monitor key alerts and dashboards for at least 30 minutes.

## Rollback Procedure
1. Stop risky rollout segment (canary or full rollout).
2. Roll back to the last stable application version.
3. Restore previous policy/config values for resilience and observability if needed.
4. Validate health, latency, and error budget stabilization.
5. Document rollback reason with correlation and trace evidence.

## Operational Drills
- Frequency:
  - Monthly: dependency timeout and circuit breaker drill.
  - Monthly: high-error-rate triage drill using correlationId/traceId.
  - Quarterly: full rollback drill including config rollback.
- Minimum evidence:
  - execution date/time, operator, scenario, detection time, mitigation time, recovery time, lessons learned.
  - include at least 3 sample `correlationId` and `traceId` pairs.
- Exit criteria:
  - incident path reproducible by another operator.
  - all required evidence completed.
  - measured recovery within SLO targets.

## Drill Record Template
- Use: `documentation/RunBooks/Drills/Observability/Observability-Drill-Execution-Template.md`
- Store completed records under: `documentation/RunBooks/Drills/Records/Observability/`

## Post-Deploy Checklist
- Use: `documentation/RunBooks/Observability/Observability-PostDeploy-Checklist.md`
- Mark each item as `PASS/FAIL/N/A` and attach evidence links.

## Verification Checklist
- `dotnet build EBOS.CRM.slnx -c Debug`
- `dotnet test tests/EBOS.CRM.ApiTests/EBOS.CRM.ApiTests.csproj -c Debug --filter "FullyQualifiedName~Observability|FullyQualifiedName~Resilience"`
- `dotnet test tests/EBOS.CRM.IntegrationTests/EBOS.CRM.IntegrationTests.csproj -c Debug --filter "FullyQualifiedName~Observability|FullyQualifiedName~Resilience"`
- confirm logs, metrics, and traces are visible in the target environment.
