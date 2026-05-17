# Mediflow HMS Foundation

This folder contains the initial production-grade foundation for Mediflow Hospital Management System.

## Stack
- Frontend: React + TypeScript + Vite + Tailwind CSS + TanStack Query/Table
- Backend: ASP.NET Core Web API + C# Clean Architecture (Api/Application/Domain/Infrastructure)
- Data: SQL Server
- Infra: Docker Compose (API, Web, SQL Server, Redis, object storage placeholder)

## Repository Layout
- `src/backend/Mediflow.slnx`
- `src/backend/Mediflow.Api`
- `src/backend/Mediflow.Application`
- `src/backend/Mediflow.Domain`
- `src/backend/Mediflow.Infrastructure`
- `src/frontend/web`

## Run with Docker
1. Copy env file:
   - `copy .env.example .env`
2. Start containers:
   - `docker compose up --build`
3. Access apps:
   - Web: `http://localhost:5173`
   - API Swagger: `http://localhost:8080/swagger`
   - Hangfire: `http://localhost:8080/hangfire`
   - MinIO Console: `http://localhost:9001`

## Run Locally (without Docker)
1. Backend:
   - `cd src/backend/Mediflow.Api`
   - `dotnet run`
2. Frontend:
   - `cd src/frontend/web`
   - `npm install`
   - `npm run dev`

## Foundation Notes
- Auth baseline and route guards are included.
- Role/permission/user domain skeleton is included.
- Business modules are intentionally deferred for next prompts.

## Deployment and Ops Readiness
- Development compose: `docker compose --env-file .env.dev.example up --build`
- Staging compose: `docker compose -f docker-compose.staging.yml --env-file .env.staging up --build -d`
- Reverse proxy config: `deploy/nginx/nginx.conf`
- Kubernetes base and overlays: `deploy/k8s/base`, `deploy/k8s/overlays/staging`, `deploy/k8s/overlays/production`
- CI/CD examples: `.github/workflows/ci.yml`, `.github/workflows/cd-example.yml`
- Deployment docs:
  - `docs/deployment/environment-strategy.md`
  - `docs/deployment/production-configuration.md`
  - `docs/deployment/logging-monitoring.md`
  - `docs/deployment/secret-management.md`

## AI Last Phase Integration
- Separate Python FastAPI service: `ai_service`
- ASP.NET AI orchestration endpoints: `/api/ai/*`
- Frontend AI review panel: `/ai/assistant`
- Contract and safety docs:
  - `docs/deployment/ai-contracts.md`
  - `docs/deployment/ai-integration.md`

## UAT and Final Refinement
- UAT package: `docs/uat/README.md`
- Performance and concurrency notes: `docs/performance_review.md`
- Production gap assessment: `docs/reviews/production_gap_assessment.md`
