from fastapi import FastAPI
from pydantic import BaseModel, Field
from typing import Dict, List

app = FastAPI(title="Mediflow AI Service", version="1.0.0")


class GenerateRequest(BaseModel):
    feature: str
    minimalContext: Dict[str, str] = Field(default_factory=dict)
    promptVersion: str
    modelVersion: str


class GenerateResponse(BaseModel):
    draftOutput: str
    safetyLabels: List[str]


def _draft_for(feature: str, context: Dict[str, str]) -> str:
    symptom = context.get("symptoms", "not provided")
    patient = context.get("patientId", "unknown-patient")

    if feature == "DoctorNoteDraft":
        return f"[DRAFT ONLY] Doctor note draft for {patient}: presenting symptoms include {symptom}. Clinical review is mandatory before finalization."
    if feature == "DischargeSummaryDraft":
        return f"[DRAFT ONLY] Discharge summary draft for {patient}. Verify diagnosis, medications, and follow-up instructions before release."
    if feature == "LabResultExplanation":
        return f"[DRAFT ONLY] Patient-friendly lab explanation for {patient}. Replace with clinician-validated interpretation."
    if feature == "DuplicatePatientDetection":
        return "[DRAFT ONLY] Potential duplicate records detected by fuzzy demographic matching. Human confirmation required."
    if feature == "BillingAnomalyDetection":
        return "[DRAFT ONLY] Potential billing anomaly flagged. Review line-items and policy rules before action."
    if feature == "InventoryDemandForecast":
        return "[DRAFT ONLY] Forecast suggests elevated demand for selected medicine SKUs next 7 days. Pharmacist approval required."

    return "[DRAFT ONLY] Unsupported feature. Human authoring required."


@app.get("/health")
def health() -> dict:
    return {"status": "ok"}


@app.post("/v1/generate", response_model=GenerateResponse)
def generate(req: GenerateRequest) -> GenerateResponse:
    return GenerateResponse(
        draftOutput=_draft_for(req.feature, req.minimalContext),
        safetyLabels=["DRAFT_ONLY", "HUMAN_REVIEW_REQUIRED", "NOT_FOR_AUTONOMOUS_USE"]
    )
