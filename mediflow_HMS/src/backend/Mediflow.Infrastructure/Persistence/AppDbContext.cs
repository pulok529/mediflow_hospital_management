using Mediflow.Application.Abstractions.Patient;
using Mediflow.Application.Abstractions.Admission;
using Mediflow.Application.Abstractions.Consultation;
using Mediflow.Application.Abstractions.Nursing;
using Microsoft.EntityFrameworkCore;

namespace Mediflow.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<RoleEntity> Roles => Set<RoleEntity>();
    public DbSet<PermissionEntity> Permissions => Set<PermissionEntity>();
    public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();
    public DbSet<AuditLogEntity> AuditLogs => Set<AuditLogEntity>();
    public DbSet<PatientEntity> Patients => Set<PatientEntity>();
    public DbSet<AppointmentEntity> Appointments => Set<AppointmentEntity>();
    public DbSet<BuildingEntity> Buildings => Set<BuildingEntity>();
    public DbSet<FloorEntity> Floors => Set<FloorEntity>();
    public DbSet<WardEntity> Wards => Set<WardEntity>();
    public DbSet<RoomEntity> Rooms => Set<RoomEntity>();
    public DbSet<BedEntity> Beds => Set<BedEntity>();
    public DbSet<AdmissionEntity> Admissions => Set<AdmissionEntity>();
    public DbSet<BedMovementEntity> BedMovements => Set<BedMovementEntity>();
    public DbSet<EncounterEntity> Encounters => Set<EncounterEntity>();
    public DbSet<EncounterVitalEntity> EncounterVitals => Set<EncounterVitalEntity>();
    public DbSet<PrescriptionItemEntity> PrescriptionItems => Set<PrescriptionItemEntity>();
    public DbSet<DoctorOrderEntity> DoctorOrders => Set<DoctorOrderEntity>();
    public DbSet<NursingNoteEntity> NursingNotes => Set<NursingNoteEntity>();
    public DbSet<MedicationAdministrationEntity> MedicationAdministrations => Set<MedicationAdministrationEntity>();
    public DbSet<VitalsScheduleEntity> VitalsSchedules => Set<VitalsScheduleEntity>();
    public DbSet<IntakeOutputEntity> IntakeOutputs => Set<IntakeOutputEntity>();
    public DbSet<ShiftHandoverEntity> ShiftHandovers => Set<ShiftHandoverEntity>();
    public DbSet<CareAlertEntity> CareAlerts => Set<CareAlertEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Email).HasMaxLength(256).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
            entity.HasMany(x => x.Roles).WithMany(x => x.Users).UsingEntity("UserRoles");
        });

        modelBuilder.Entity<RoleEntity>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(128).IsRequired();
            entity.HasMany(x => x.Permissions).WithMany(x => x.Roles).UsingEntity("RolePermissions");
        });

        modelBuilder.Entity<PermissionEntity>(entity =>
        {
            entity.ToTable("Permissions");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Code).IsUnique();
            entity.Property(x => x.Code).HasMaxLength(128).IsRequired();
        });

        modelBuilder.Entity<RefreshTokenEntity>(entity =>
        {
            entity.ToTable("RefreshTokens");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Token).IsUnique();
            entity.Property(x => x.Token).HasMaxLength(512).IsRequired();
        });

        modelBuilder.Entity<AuditLogEntity>(entity =>
        {
            entity.ToTable("AuditLogs");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.AtUtc);
            entity.HasIndex(x => new { x.EntityType, x.EntityId });
            entity.Property(x => x.Action).HasMaxLength(128).IsRequired();
            entity.Property(x => x.EntityType).HasMaxLength(128).IsRequired();
            entity.Property(x => x.EntityId).HasMaxLength(128).IsRequired();
            entity.Property(x => x.Details).HasMaxLength(2000).IsRequired();
        });

        modelBuilder.Entity<PatientEntity>(entity =>
        {
            entity.ToTable("Patients");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.PatientCode).IsUnique();
            entity.HasIndex(x => x.Phone);
            entity.HasIndex(x => new { x.FullName, x.DateOfBirth });
            entity.Property(x => x.PatientCode).HasMaxLength(64).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Phone).HasMaxLength(32).IsRequired();
            entity.Property(x => x.Gender).HasMaxLength(32).IsRequired();
            entity.Property(x => x.Address).HasMaxLength(500).IsRequired();
        });

        modelBuilder.Entity<AppointmentEntity>(entity =>
        {
            entity.ToTable("Appointments");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.AppointmentDate, x.Department, x.DoctorName, x.TokenNumber }).IsUnique();
            entity.Property(x => x.PatientName).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Department).HasMaxLength(128).IsRequired();
            entity.Property(x => x.DoctorName).HasMaxLength(128).IsRequired();
            entity.HasOne(x => x.Patient).WithMany(x => x.Appointments).HasForeignKey(x => x.PatientId);
            entity.Property(x => x.State).HasConversion<string>().HasMaxLength(32);
            entity.Property(x => x.QueueState).HasConversion<string>().HasMaxLength(32);
        });

        modelBuilder.Entity<BuildingEntity>(entity =>
        {
            entity.ToTable("Buildings");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Name).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(128).IsRequired();
        });

        modelBuilder.Entity<FloorEntity>(entity =>
        {
            entity.ToTable("Floors");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.BuildingId, x.Name }).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(128).IsRequired();
            entity.HasOne(x => x.Building).WithMany(x => x.Floors).HasForeignKey(x => x.BuildingId);
        });

        modelBuilder.Entity<WardEntity>(entity =>
        {
            entity.ToTable("Wards");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.FloorId, x.Name }).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(128).IsRequired();
            entity.HasOne(x => x.Floor).WithMany(x => x.Wards).HasForeignKey(x => x.FloorId);
        });

        modelBuilder.Entity<RoomEntity>(entity =>
        {
            entity.ToTable("Rooms");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.WardId, x.Name }).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(128).IsRequired();
            entity.HasOne(x => x.Ward).WithMany(x => x.Rooms).HasForeignKey(x => x.WardId);
        });

        modelBuilder.Entity<BedEntity>(entity =>
        {
            entity.ToTable("Beds");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.RoomId, x.BedNumber }).IsUnique();
            entity.Property(x => x.BedNumber).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(64);
            entity.Property(x => x.RowVersion).IsRowVersion();
            entity.HasOne(x => x.Room).WithMany(x => x.Beds).HasForeignKey(x => x.RoomId);
        });

        modelBuilder.Entity<AdmissionEntity>(entity =>
        {
            entity.ToTable("Admissions");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.PatientId, x.Status })
                .IsUnique()
                .HasFilter("[Status] IN ('Admitted', 'Requested')");
            entity.HasIndex(x => new { x.CurrentBedId, x.Status })
                .IsUnique()
                .HasFilter("[CurrentBedId] IS NOT NULL AND [Status] = 'Admitted'");
            entity.Property(x => x.PatientName).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Source).HasMaxLength(128).IsRequired();
            entity.Property(x => x.DepositAmount).HasPrecision(18, 2);
            entity.Property(x => x.DepositReference).HasMaxLength(128);
            entity.Property(x => x.Status).HasMaxLength(64).IsRequired();
            entity.Property(x => x.RowVersion).IsRowVersion();
            entity.HasOne(x => x.CurrentBed).WithMany().HasForeignKey(x => x.CurrentBedId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BedMovementEntity>(entity =>
        {
            entity.ToTable("BedMovements");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.AdmissionId);
            entity.Property(x => x.Reason).HasMaxLength(500).IsRequired();
            entity.HasOne(x => x.Admission).WithMany(x => x.BedMovements).HasForeignKey(x => x.AdmissionId);
            entity.HasOne(x => x.FromBed).WithMany().HasForeignKey(x => x.FromBedId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.ToBed).WithMany().HasForeignKey(x => x.ToBedId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EncounterEntity>(entity =>
        {
            entity.ToTable("Encounters");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.AppointmentId, x.State })
                .IsUnique()
                .HasFilter("[State] = 'Open'");
            entity.HasIndex(x => x.PatientId);
            entity.Property(x => x.PatientName).HasMaxLength(256).IsRequired();
            entity.Property(x => x.State).HasConversion<string>().HasMaxLength(32);
            entity.Property(x => x.ChiefComplaint).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.Diagnosis).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.ClinicalNotes).HasMaxLength(4000).IsRequired();
            entity.Property(x => x.FollowUpAdvice).HasMaxLength(2000).IsRequired();
            entity.Property(x => x.RowVersion).IsRowVersion();
        });

        modelBuilder.Entity<EncounterVitalEntity>(entity =>
        {
            entity.ToTable("EncounterVitals");
            entity.HasKey(x => x.EncounterId);
            entity.HasOne(x => x.Encounter).WithOne(x => x.Vitals).HasForeignKey<EncounterVitalEntity>(x => x.EncounterId);
            entity.Property(x => x.TemperatureC).HasPrecision(5, 2);
            entity.Property(x => x.SpO2).HasPrecision(5, 2);
            entity.Property(x => x.WeightKg).HasPrecision(6, 2);
            entity.Property(x => x.HeightCm).HasPrecision(6, 2);
        });

        modelBuilder.Entity<PrescriptionItemEntity>(entity =>
        {
            entity.ToTable("PrescriptionItems");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.EncounterId);
            entity.Property(x => x.MedicineName).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Dose).HasMaxLength(128).IsRequired();
            entity.Property(x => x.Frequency).HasMaxLength(128).IsRequired();
            entity.Property(x => x.Duration).HasMaxLength(128).IsRequired();
            entity.Property(x => x.Instructions).HasMaxLength(1000).IsRequired();
            entity.HasOne(x => x.Encounter).WithMany(x => x.Prescriptions).HasForeignKey(x => x.EncounterId);
        });

        modelBuilder.Entity<DoctorOrderEntity>(entity =>
        {
            entity.ToTable("DoctorOrders");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.EncounterId);
            entity.Property(x => x.Type).HasConversion<string>().HasMaxLength(32);
            entity.Property(x => x.TestOrProcedure).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Notes).HasMaxLength(1000).IsRequired();
            entity.HasOne(x => x.Encounter).WithMany(x => x.Orders).HasForeignKey(x => x.EncounterId);
        });

        modelBuilder.Entity<NursingNoteEntity>(entity =>
        {
            entity.ToTable("NursingNotes");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.AdmissionId, x.AtUtc });
            entity.Property(x => x.NurseName).HasMaxLength(128).IsRequired();
            entity.Property(x => x.Note).HasMaxLength(4000).IsRequired();
            entity.HasOne(x => x.Admission).WithMany().HasForeignKey(x => x.AdmissionId);
            entity.HasOne(x => x.LinkedDoctorRoundEncounter).WithMany().HasForeignKey(x => x.LinkedDoctorRoundEncounterId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MedicationAdministrationEntity>(entity =>
        {
            entity.ToTable("MedicationAdministrations");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.Status, x.DueAtUtc });
            entity.HasIndex(x => x.AdmissionId);
            entity.Property(x => x.MedicationName).HasMaxLength(256).IsRequired();
            entity.Property(x => x.Dose).HasMaxLength(128).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(32);
            entity.Property(x => x.Remarks).HasMaxLength(1000);
            entity.Property(x => x.RowVersion).IsRowVersion();
            entity.HasOne(x => x.Admission).WithMany().HasForeignKey(x => x.AdmissionId);
        });

        modelBuilder.Entity<VitalsScheduleEntity>(entity =>
        {
            entity.ToTable("VitalsSchedules");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.AdmissionId, x.DueAtUtc });
            entity.Property(x => x.TemperatureC).HasPrecision(5, 2);
            entity.Property(x => x.SpO2).HasPrecision(5, 2);
            entity.Property(x => x.RowVersion).IsRowVersion();
            entity.HasOne(x => x.Admission).WithMany().HasForeignKey(x => x.AdmissionId);
        });

        modelBuilder.Entity<IntakeOutputEntity>(entity =>
        {
            entity.ToTable("IntakeOutputs");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.AdmissionId, x.AtUtc });
            entity.Property(x => x.IntakeMl).HasPrecision(10, 2);
            entity.Property(x => x.OutputMl).HasPrecision(10, 2);
            entity.Property(x => x.Notes).HasMaxLength(1000).IsRequired();
            entity.HasOne(x => x.Admission).WithMany().HasForeignKey(x => x.AdmissionId);
        });

        modelBuilder.Entity<ShiftHandoverEntity>(entity =>
        {
            entity.ToTable("ShiftHandovers");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.AdmissionId, x.AtUtc });
            entity.Property(x => x.FromShift).HasMaxLength(64).IsRequired();
            entity.Property(x => x.ToShift).HasMaxLength(64).IsRequired();
            entity.Property(x => x.Summary).HasMaxLength(4000).IsRequired();
            entity.HasOne(x => x.Admission).WithMany().HasForeignKey(x => x.AdmissionId);
        });

        modelBuilder.Entity<CareAlertEntity>(entity =>
        {
            entity.ToTable("CareAlerts");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.Resolved, x.AtUtc });
            entity.HasIndex(x => x.AdmissionId);
            entity.Property(x => x.Severity).HasConversion<string>().HasMaxLength(32);
            entity.Property(x => x.Message).HasMaxLength(1000).IsRequired();
            entity.HasOne(x => x.Admission).WithMany().HasForeignKey(x => x.AdmissionId);
        });
    }
}

public sealed class UserEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public List<RoleEntity> Roles { get; set; } = [];
}

public sealed class RoleEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public List<UserEntity> Users { get; set; } = [];
    public List<PermissionEntity> Permissions { get; set; } = [];
}

public sealed class PermissionEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public List<RoleEntity> Roles { get; set; } = [];
}

public sealed class RefreshTokenEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public bool Revoked { get; set; }
}

public sealed class AuditLogEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? ActorUserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public DateTime AtUtc { get; set; } = DateTime.UtcNow;
    public string Details { get; set; } = string.Empty;
}

public sealed class PatientEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string PatientCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public Guid? UpdatedBy { get; set; }
    public List<AppointmentEntity> Appointments { get; set; } = [];
}

public sealed class AppointmentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientId { get; set; }
    public PatientEntity Patient { get; set; } = null!;
    public string PatientName { get; set; } = string.Empty;
    public DateOnly AppointmentDate { get; set; }
    public string Department { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public AppointmentState State { get; set; } = AppointmentState.Scheduled;
    public int TokenNumber { get; set; }
    public QueueState QueueState { get; set; } = QueueState.Waiting;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public Guid? UpdatedBy { get; set; }
}

public sealed class BuildingEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public List<FloorEntity> Floors { get; set; } = [];
}

public sealed class FloorEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BuildingId { get; set; }
    public BuildingEntity Building { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public List<WardEntity> Wards { get; set; } = [];
}

public sealed class WardEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid FloorId { get; set; }
    public FloorEntity Floor { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public List<RoomEntity> Rooms { get; set; } = [];
}

public sealed class RoomEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WardId { get; set; }
    public WardEntity Ward { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public List<BedEntity> Beds { get; set; } = [];
}

public sealed class BedEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RoomId { get; set; }
    public RoomEntity Room { get; set; } = null!;
    public string BedNumber { get; set; } = string.Empty;
    public BedStatus Status { get; set; } = BedStatus.Available;
    public byte[] RowVersion { get; set; } = [];
}

public sealed class AdmissionEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? AdmittedAtUtc { get; set; }
    public decimal DepositAmount { get; set; }
    public string? DepositReference { get; set; }
    public Guid? CurrentBedId { get; set; }
    public BedEntity? CurrentBed { get; set; }
    public string Status { get; set; } = "Requested";
    public byte[] RowVersion { get; set; } = [];
    public List<BedMovementEntity> BedMovements { get; set; } = [];
}

public sealed class BedMovementEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AdmissionId { get; set; }
    public AdmissionEntity Admission { get; set; } = null!;
    public Guid? FromBedId { get; set; }
    public BedEntity? FromBed { get; set; }
    public Guid ToBedId { get; set; }
    public BedEntity ToBed { get; set; } = null!;
    public DateTime MovedAtUtc { get; set; } = DateTime.UtcNow;
    public string Reason { get; set; } = string.Empty;
}

public sealed class EncounterEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; set; }
    public EncounterState State { get; set; } = EncounterState.Open;
    public string ChiefComplaint { get; set; } = string.Empty;
    public string Diagnosis { get; set; } = string.Empty;
    public string ClinicalNotes { get; set; } = string.Empty;
    public DateOnly? FollowUpDate { get; set; }
    public string FollowUpAdvice { get; set; } = string.Empty;
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public byte[] RowVersion { get; set; } = [];
    public EncounterVitalEntity? Vitals { get; set; }
    public List<PrescriptionItemEntity> Prescriptions { get; set; } = [];
    public List<DoctorOrderEntity> Orders { get; set; } = [];
}

public sealed class EncounterVitalEntity
{
    public Guid EncounterId { get; set; }
    public EncounterEntity Encounter { get; set; } = null!;
    public decimal? TemperatureC { get; set; }
    public int? Pulse { get; set; }
    public int? SystolicBp { get; set; }
    public int? DiastolicBp { get; set; }
    public int? RespiratoryRate { get; set; }
    public decimal? SpO2 { get; set; }
    public decimal? WeightKg { get; set; }
    public decimal? HeightCm { get; set; }
}

public sealed class PrescriptionItemEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EncounterId { get; set; }
    public EncounterEntity Encounter { get; set; } = null!;
    public string MedicineName { get; set; } = string.Empty;
    public string Dose { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
}

public sealed class DoctorOrderEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EncounterId { get; set; }
    public EncounterEntity Encounter { get; set; } = null!;
    public OrderType Type { get; set; }
    public string TestOrProcedure { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public sealed class NursingNoteEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AdmissionId { get; set; }
    public AdmissionEntity Admission { get; set; } = null!;
    public DateTime AtUtc { get; set; } = DateTime.UtcNow;
    public string NurseName { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public Guid? LinkedDoctorRoundEncounterId { get; set; }
    public EncounterEntity? LinkedDoctorRoundEncounter { get; set; }
    public Guid? CreatedBy { get; set; }
}

public sealed class MedicationAdministrationEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AdmissionId { get; set; }
    public AdmissionEntity Admission { get; set; } = null!;
    public string MedicationName { get; set; } = string.Empty;
    public string Dose { get; set; } = string.Empty;
    public DateTime DueAtUtc { get; set; }
    public DateTime? AdministeredAtUtc { get; set; }
    public MedicationStatus Status { get; set; } = MedicationStatus.Due;
    public string? Remarks { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? AdministeredBy { get; set; }
    public byte[] RowVersion { get; set; } = [];
}

public sealed class VitalsScheduleEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AdmissionId { get; set; }
    public AdmissionEntity Admission { get; set; } = null!;
    public DateTime DueAtUtc { get; set; }
    public decimal? TemperatureC { get; set; }
    public int? Pulse { get; set; }
    public int? SystolicBp { get; set; }
    public int? DiastolicBp { get; set; }
    public decimal? SpO2 { get; set; }
    public bool Recorded { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? RecordedBy { get; set; }
    public byte[] RowVersion { get; set; } = [];
}

public sealed class IntakeOutputEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AdmissionId { get; set; }
    public AdmissionEntity Admission { get; set; } = null!;
    public DateTime AtUtc { get; set; } = DateTime.UtcNow;
    public decimal IntakeMl { get; set; }
    public decimal OutputMl { get; set; }
    public string Notes { get; set; } = string.Empty;
    public Guid? CreatedBy { get; set; }
}

public sealed class ShiftHandoverEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AdmissionId { get; set; }
    public AdmissionEntity Admission { get; set; } = null!;
    public DateTime AtUtc { get; set; } = DateTime.UtcNow;
    public string FromShift { get; set; } = string.Empty;
    public string ToShift { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public Guid? CreatedBy { get; set; }
}

public sealed class CareAlertEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AdmissionId { get; set; }
    public AdmissionEntity Admission { get; set; } = null!;
    public AlertSeverity Severity { get; set; } = AlertSeverity.Info;
    public string Message { get; set; } = string.Empty;
    public DateTime AtUtc { get; set; } = DateTime.UtcNow;
    public bool Resolved { get; set; }
}
