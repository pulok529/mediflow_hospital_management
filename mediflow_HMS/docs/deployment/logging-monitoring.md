# Logging and Monitoring Integration Points

## Application Logs
- API logs to stdout (container native)
- Collect logs through Fluent Bit / OpenTelemetry collector
- Ship to ELK, Loki, or cloud logging backend

## Metrics
- Add OpenTelemetry metrics exporter in API for request latency, error rate, and throughput
- Track frontend availability and web vitals using synthetic monitoring

## Health Checks
- API probe path: `/health`
- Web probe path: `/healthz`

## Alerts
- 5xx error rate threshold
- High p95 latency threshold
- Pod restart anomalies
- Database connection failure alerts
