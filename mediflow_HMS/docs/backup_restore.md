# Backup and Restore Operations

## SQL Server Backup
- Daily full backup schedule recommended.
- Keep 7 daily + 4 weekly backups.
- Validate restore in staging weekly.

Example command:
`BACKUP DATABASE MediflowDb TO DISK='C:\backups\MediflowDb_full.bak' WITH INIT, COMPRESSION;`

## SQL Server Restore
1. Stop API write traffic.
2. Restore latest full backup.
3. Restore differential/log backups if used.
4. Run smoke checks (`/healthz`, auth, billing, admissions).

Example:
`RESTORE DATABASE MediflowDb FROM DISK='C:\backups\MediflowDb_full.bak' WITH REPLACE;`

## File/Object Storage
- Backup MinIO object buckets (imaging refs metadata points here).
- Validate object path integrity post-restore.

## App Recovery Checklist
- `docker compose up -d`
- Verify `http://localhost:8080/healthz`
- Verify SignalR `/hubs/notifications`
- Verify Hangfire dashboard `/hangfire`
