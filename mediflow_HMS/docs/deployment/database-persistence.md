# Database Persistence

This phase starts the move from demo in-memory services to SQL Server persistence.

## Implemented slice
- EF Core `AppDbContext`
- SQL-backed users, roles, permissions, refresh tokens
- SQL-backed audit logs
- SQL-backed patients
- SQL-backed appointments and queue state
- SQL-backed admission/facility hierarchy: buildings, floors, wards, rooms, beds, admissions, bed movements
- SQL-backed consultations: encounters, vitals, prescriptions, and doctor lab/radiology orders
- SQL-backed Nursing/IPD care: notes, medication administration, vitals schedules, intake-output, shift handovers, care alerts
- Database guards for active admission per patient and active bed assignment
- Consultation guards for one open encounter per appointment and locked completed encounters
- Nursing guards for locked administered medications and recorded vitals schedules
- EF migrations:
  - `InitialPersistenceSlice`
  - `AdmissionBedPersistence`
  - `ConsultationPrescriptionPersistence`
  - `NursingIpdPersistence`

## Apply migration locally
Run SQL Server from Docker first:

```powershell
docker compose --env-file .env.dev.example up -d sqlserver
```

Then apply migrations:

```powershell
dotnet ef database update --project src/backend/Mediflow.Infrastructure/Mediflow.Infrastructure.csproj --startup-project src/backend/Mediflow.Api/Mediflow.Api.csproj --context AppDbContext
```

## Next persistence slices
1. Diagnostics.
2. Pharmacy stock ledger.
3. Billing and discharge.
4. AI request and approval history.
