# Mediflow AI Service

Standalone Python FastAPI service for last-phase AI drafting.

## Endpoints
- `GET /health`
- `POST /v1/generate`

All responses are draft-only and require human approval in the ASP.NET system.

## Run locally
```bash
pip install -r requirements.txt
uvicorn main:app --host 0.0.0.0 --port 8090
```
