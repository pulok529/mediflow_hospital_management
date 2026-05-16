# Production Configuration Guidance

## Reverse Proxy and TLS
- Terminate TLS at ingress or edge load balancer
- Route `/` to web and `/api`, `/hubs` to api
- Enable HSTS and secure headers at edge

## Scaling
- API and web are stateless: scale horizontally
- Keep SQL Server and Redis as managed services in production

## Database
- Enable automated backups and point-in-time recovery
- Use migration job during release pipeline

## Security Baseline
- Run containers with least privilege
- Rotate JWT secret periodically
- Restrict network access to db/redis to private subnets

## Release Process
- Build immutable images per tag
- Promote same image across staging -> production
- Rollback by redeploying last known good image tag
