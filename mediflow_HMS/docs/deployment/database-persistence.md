# Database Persistence

This phase starts the move from demo in-memory services to SQL Server persistence.

## Implemented slice
- EF Core `AppDbContext`
- SQL-backed users, roles, permissions, refresh tokens
- SQL-backed audit logs
- SQL-backed patients
- SQL-backed appointments and queue state
- Initial EF migration: `InitialPersistenceSlice`

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
1. Admission and bed management.
2. Consultation and prescriptions.
3. Nursing and IPD care.
4. Diagnostics.
5. Pharmacy stock ledger.
6. Billing and discharge.
7. AI request and approval history.
