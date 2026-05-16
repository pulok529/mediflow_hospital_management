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
