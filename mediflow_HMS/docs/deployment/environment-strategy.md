# Environment Strategy

## Files
- `.env.dev.example`: local development template
- `.env.staging.example`: staging variable template
- `.env.prod.example`: production variable template

## Rules
- Never commit real production secrets
- Inject production values through CI/CD secret store
- Keep non-secret config in appsettings and env vars for overrides

## Runtime Mapping
- `JWT_SECRET` -> `Jwt__Secret`
- `SQLSERVER_SA_PASSWORD` used to construct SQL connection string
- `MINIO_ROOT_USER` / `MINIO_ROOT_PASSWORD` for object storage

## Local Dev
`docker compose --env-file .env.dev.example up -d`

## Staging
`docker compose --env-file .env.staging up -f docker-compose.staging.yml up -d`
