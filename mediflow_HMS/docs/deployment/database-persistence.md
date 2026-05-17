# Database Persistence

This phase starts the move from demo in-memory services to SQL Server persistence.

## Implemented slice
- EF Core `AppDbContext`
- SQL-backed users, roles, permissions, refresh tokens
- SQL-backed audit logs
- SQL-backed patients
- SQL-backed appointments and queue state
- SQL-backed admission/facility hierarchy: buildings, floors, wards, rooms, beds, admissions, bed movements
- Database guards for active admission per patient and active bed assignment
- EF migrations:
  - `InitialPersistenceSlice`
  - `AdmissionBedPersistence`

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
1. Consultation and prescriptions.
2. Nursing and IPD care.
3. Diagnostics.
4. Pharmacy stock ledger.
5. Billing and discharge.
6. AI request and approval history.
