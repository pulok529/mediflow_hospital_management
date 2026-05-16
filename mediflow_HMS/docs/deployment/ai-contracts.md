# AI API Contracts (ASP.NET <-> FastAPI)

## ASP.NET to FastAPI
`POST /v1/generate`
```json
{
  "feature": "DoctorNoteDraft",
  "minimalContext": {
    "patientId": "P-101",
    "encounterId": "E-21"
  },
  "promptVersion": "v1.0.0",
  "modelVersion": "mediflow-safe-draft-v1"
}
```

## FastAPI response
```json
{
  "draftOutput": "[DRAFT ONLY] ...",
  "safetyLabels": ["DRAFT_ONLY", "HUMAN_REVIEW_REQUIRED", "NOT_FOR_AUTONOMOUS_USE"]
}
```

## ASP.NET AI orchestration API
- `POST /api/ai/drafts`
- `GET /api/ai/drafts`
- `GET /api/ai/drafts/{requestId}`
- `POST /api/ai/drafts/{requestId}/review`

Review endpoint updates approval status and appends audit trail.
