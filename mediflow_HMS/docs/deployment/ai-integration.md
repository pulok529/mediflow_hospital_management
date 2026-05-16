# AI Integration Notes

- AI is deployed as a separate Python FastAPI service (`ai_service`).
- Main hospital domain logic remains in ASP.NET Core.
- ASP.NET sends only minimal context fields to AI service.
- Every output is labeled draft-only and must be approved/rejected by a human user.
- Stored metadata includes request, output, prompt version, model version, approval status, reviewer notes, and audit trail entries.

## Initial AI Features Implemented
1. Doctor note drafting
2. Discharge summary generation
3. Patient-friendly lab result explanation
4. Duplicate patient detection
5. Billing anomaly detection
6. Inventory demand forecasting
