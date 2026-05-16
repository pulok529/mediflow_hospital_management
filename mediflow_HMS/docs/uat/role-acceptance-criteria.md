# Role-by-Role Acceptance Criteria

## SuperAdmin/Admin
- Can manage users, roles, and permissions
- Can access reporting center and audit explorer
- Can review AI drafts and approve/reject

## Reception
- Can register/search patients and book appointments
- Can manage daily queue states
- Cannot access admin-only controls

## Doctor
- Can start/save consultation and complete encounter
- Can view patient history
- Can request and review AI doctor-note/discharge drafts

## Nurse
- Can manage notes, vitals, medication administration, handover
- Can access transfer/discharge prep list

## Lab Technician / Radiologist
- Can manage diagnostics catalog and orders
- Can collect/enter/approve test outputs per role policy
- Can generate patient-friendly lab explanation drafts for review

## Pharmacist
- Can manage medicines, batches, stock movement, low-stock alerts
- Can run inventory demand forecast drafts and review outputs

## Billing Clerk
- Can manage bills, payments, refunds, discount workflows
- Can run billing anomaly detection drafts and complete human review

## Cross-Role Quality Gate
- Error messages are understandable and actionable
- Role-inappropriate actions return clear access-denied behavior
