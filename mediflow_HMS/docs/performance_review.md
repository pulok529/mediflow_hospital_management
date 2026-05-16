# Performance Review Notes

## Read-heavy Optimization
- Introduce Dapper for report-heavy read paths.
- Keep aggregate queries projection-only.
- Prefer server-side filtering for report center datasets.

## Runtime Hardening
- Global rate limiter enabled.
- Security headers enabled.
- Structured request logs + exception handler.
- Health checks exposed at `/healthz`.

## Next Optimization Targets
1. Replace in-memory stores with SQL + indexes.
2. Add Redis cache for dashboard KPI snapshots.
3. Add pagination for audit and report datasets.

## Concurrency Review
- In-memory services are suitable for demo/UAT only and are not safe for multi-instance scale-out.
- Production path should move write workflows to SQL transactions with row/version concurrency checks.
- Queue-sensitive operations (token assignment, bed assignment, discount approval) must use atomic persistence semantics.
- Add load test scenarios for:
  1. Concurrent appointment booking.
  2. Concurrent bed transfer/assignment.
  3. Concurrent billing line updates and payment posting.
