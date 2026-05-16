# End-to-End Workflow Testing Checklist

## 1. Reception -> Appointment -> Doctor
- Register patient with mandatory fields
- Search and confirm patient appears in lookup
- Book appointment and verify token generation
- Verify queue state transitions (Waiting -> InConsultation -> Completed)

## 2. Doctor -> Diagnostics -> Reporting
- Start encounter from waiting list
- Save diagnosis, notes, prescriptions, orders
- Create lab and imaging orders
- Enter and approve results
- Confirm data is visible in reporting views

## 3. Admission -> Nursing -> Transfer/Discharge
- Admit patient and assign bed
- Record nursing notes, vitals, medications
- Transfer bed and verify movement history
- Prepare discharge workflow

## 4. Pharmacy -> Billing -> Final Clearance
- Dispense medication and verify stock movement
- Create OPD/IPD bills and add running lines
- Process payment, discount request, refund path
- Complete final bill clearance and discharge summary

## 5. AI Draft -> Human Review
- Create draft for each AI feature
- Confirm safety labels are shown
- Approve and reject drafts
- Verify audit trail entries for request and review actions

## 6. Security and Access
- Validate role-restricted pages/actions
- Validate unauthorized API request behavior
- Verify audit logs for sensitive operations

## Exit Criteria
- All critical workflows pass with no blocker defects
- Medium defects have documented workaround
- All UAT defects are triaged and ownership assigned
