# Secure Secret Management Strategy

## Secret Sources
- Development: `.env.dev` only
- Staging/Production: GitHub Actions secrets + Kubernetes secrets
- Enterprise target: HashiCorp Vault or cloud secret manager + External Secrets Operator

## Handling Rules
- Never store plaintext secrets in git
- Rotate high-impact secrets (JWT, DB credentials) at least every 90 days
- Use distinct secrets per environment
- Audit secret access via CI/CD and cloud IAM logs

## Kubernetes Pattern
- Keep secret manifests generated at deploy-time
- Prefer sealed secrets or external secret syncing for GitOps
