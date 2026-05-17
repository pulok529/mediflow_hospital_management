using Mediflow.Application.Abstractions.Patient;
using Mediflow.Application.Abstractions.Admission;
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
