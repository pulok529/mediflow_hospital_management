using Mediflow.Application.Abstractions.Admission;
using Mediflow.Application.Abstractions.Audit;
using Mediflow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mediflow.Infrastructure.Admission;

internal sealed class DatabaseAdmissionWorkflowService(AppDbContext db, IAuditLogService auditLogService) : IAdmissionWorkflowService
{
    public BuildingDto CreateBuilding(CreateBuildingRequest request, Guid? actorUserId)
    {
        var entity = new BuildingEntity { Name = request.Name };
        db.Buildings.Add(entity);
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Building.Create", "Building", entity.Id.ToString(), entity.Name);
        return new BuildingDto(entity.Id, entity.Name);
    }

    public FloorDto CreateFloor(CreateFloorRequest request, Guid? actorUserId)
    {
        if (!db.Buildings.Any(x => x.Id == request.BuildingId))
            throw new InvalidOperationException("Building not found.");

        var entity = new FloorEntity { BuildingId = request.BuildingId, Name = request.Name };
        db.Floors.Add(entity);
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Floor.Create", "Floor", entity.Id.ToString(), entity.Name);
        return new FloorDto(entity.Id, entity.BuildingId, entity.Name);
    }

    public WardDto CreateWard(CreateWardRequest request, Guid? actorUserId)
    {
        if (!db.Floors.Any(x => x.Id == request.FloorId))
            throw new InvalidOperationException("Floor not found.");

        var entity = new WardEntity { FloorId = request.FloorId, Name = request.Name };
        db.Wards.Add(entity);
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Ward.Create", "Ward", entity.Id.ToString(), entity.Name);
        return new WardDto(entity.Id, entity.FloorId, entity.Name);
    }

    public RoomDto CreateRoom(CreateRoomRequest request, Guid? actorUserId)
    {
        if (!db.Wards.Any(x => x.Id == request.WardId))
            throw new InvalidOperationException("Ward not found.");

        var entity = new RoomEntity { WardId = request.WardId, Name = request.Name };
        db.Rooms.Add(entity);
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Room.Create", "Room", entity.Id.ToString(), entity.Name);
        return new RoomDto(entity.Id, entity.WardId, entity.Name);
    }

    public BedDto CreateBed(CreateBedRequest request, Guid? actorUserId)
    {
        if (!db.Rooms.Any(x => x.Id == request.RoomId))
            throw new InvalidOperationException("Room not found.");

        var entity = new BedEntity { RoomId = request.RoomId, BedNumber = request.BedNumber, Status = BedStatus.Available };
        db.Beds.Add(entity);
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Bed.Create", "Bed", entity.Id.ToString(), entity.BedNumber);
        return MapBed(entity);
    }

    public BedDto? UpdateBedStatus(Guid bedId, UpdateBedStatusRequest request, Guid? actorUserId)
    {
        var bed = db.Beds.FirstOrDefault(x => x.Id == bedId);
        if (bed is null) return null;

        bed.Status = request.Status;
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Bed.Status", "Bed", bed.Id.ToString(), bed.Status.ToString());
        return MapBed(bed);
    }

    public IReadOnlyCollection<BuildingDto> GetBuildings()
        => db.Buildings.AsNoTracking().OrderBy(x => x.Name).Select(x => new BuildingDto(x.Id, x.Name)).ToArray();

    public IReadOnlyCollection<FloorDto> GetFloors()
        => db.Floors.AsNoTracking().OrderBy(x => x.Name).Select(x => new FloorDto(x.Id, x.BuildingId, x.Name)).ToArray();

    public IReadOnlyCollection<WardDto> GetWards()
        => db.Wards.AsNoTracking().OrderBy(x => x.Name).Select(x => new WardDto(x.Id, x.FloorId, x.Name)).ToArray();

    public IReadOnlyCollection<RoomDto> GetRooms()
        => db.Rooms.AsNoTracking().OrderBy(x => x.Name).Select(x => new RoomDto(x.Id, x.WardId, x.Name)).ToArray();

    public IReadOnlyCollection<BedDto> GetBeds()
        => db.Beds.AsNoTracking().OrderBy(x => x.BedNumber).Select(x => MapBed(x)).ToArray();

    public AdmissionDto CreateAdmissionRequest(CreateAdmissionRequest request, Guid? actorUserId)
    {
        var hasActiveAdmission = db.Admissions.Any(x =>
            x.PatientId == request.PatientId &&
            (x.Status == "Requested" || x.Status == "Admitted"));

        if (hasActiveAdmission)
            throw new InvalidOperationException("Patient already has an active admission request or admission.");

        var admission = new AdmissionEntity
        {
            PatientId = request.PatientId,
            PatientName = request.PatientName,
            Source = request.Source,
            RequestedAtUtc = DateTime.UtcNow,
            DepositAmount = request.DepositAmount,
            DepositReference = request.DepositReference,
            Status = "Requested"
        };

        db.Admissions.Add(admission);
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Admission.Request", "Admission", admission.Id.ToString(), $"Source={request.Source}");
        return MapAdmission(admission);
    }

    public AdmissionDto? AssignBed(Guid admissionId, AssignBedRequest request, Guid? actorUserId)
    {
        using var transaction = db.Database.BeginTransaction();
        var admission = db.Admissions.FirstOrDefault(x => x.Id == admissionId);
        var bed = db.Beds.FirstOrDefault(x => x.Id == request.BedId);
        if (admission is null || bed is null) return null;
        if (admission.Status == "Admitted")
            throw new InvalidOperationException("Admission already has an assigned bed.");
        if (bed.Status is not (BedStatus.Available or BedStatus.Reserved))
            throw new InvalidOperationException("Bed is not assignable.");

        var bedOccupied = db.Admissions.Any(x => x.CurrentBedId == bed.Id && x.Status == "Admitted");
        if (bedOccupied)
            throw new InvalidOperationException("Bed already has an active admission.");

        bed.Status = BedStatus.Occupied;
        admission.CurrentBedId = bed.Id;
        admission.AdmittedAtUtc = DateTime.UtcNow;
        admission.Status = "Admitted";

        var movement = new BedMovementEntity
        {
            AdmissionId = admission.Id,
            FromBedId = null,
            ToBedId = bed.Id,
            MovedAtUtc = DateTime.UtcNow,
            Reason = "Initial assignment"
        };
        db.BedMovements.Add(movement);
        db.SaveChanges();
        transaction.Commit();

        auditLogService.Add(actorUserId, "Bed.Assign", "Admission", admission.Id.ToString(), $"Bed={bed.BedNumber}");
        auditLogService.Add(actorUserId, "Bed.Movement", "BedMovement", movement.Id.ToString(), "Initial assignment");
        return MapAdmission(admission);
    }

    public AdmissionDto? TransferBed(Guid admissionId, TransferBedRequest request, Guid? actorUserId)
    {
        using var transaction = db.Database.BeginTransaction();
        var admission = db.Admissions.FirstOrDefault(x => x.Id == admissionId);
        var toBed = db.Beds.FirstOrDefault(x => x.Id == request.ToBedId);
        if (admission is null || toBed is null || admission.CurrentBedId is null) return null;
        if (toBed.Status is not (BedStatus.Available or BedStatus.Reserved))
            throw new InvalidOperationException("Target bed is not assignable.");

        var toBedOccupied = db.Admissions.Any(x => x.CurrentBedId == toBed.Id && x.Status == "Admitted");
        if (toBedOccupied)
            throw new InvalidOperationException("Target bed already has an active admission.");

        var fromBed = db.Beds.First(x => x.Id == admission.CurrentBedId.Value);
        fromBed.Status = BedStatus.CleaningRequired;
        toBed.Status = BedStatus.Occupied;
        admission.CurrentBedId = toBed.Id;

        var movement = new BedMovementEntity
        {
            AdmissionId = admission.Id,
            FromBedId = fromBed.Id,
            ToBedId = toBed.Id,
            MovedAtUtc = DateTime.UtcNow,
            Reason = request.Reason
        };
        db.BedMovements.Add(movement);
        db.SaveChanges();
        transaction.Commit();

        auditLogService.Add(actorUserId, "Bed.Transfer", "Admission", admission.Id.ToString(), $"{fromBed.BedNumber}->{toBed.BedNumber}");
        auditLogService.Add(actorUserId, "Bed.Movement", "BedMovement", movement.Id.ToString(), request.Reason);
        return MapAdmission(admission);
    }

    public IReadOnlyCollection<AdmissionDto> GetAdmissions()
        => db.Admissions.AsNoTracking().OrderByDescending(x => x.RequestedAtUtc).Select(x => MapAdmission(x)).ToArray();

    public IReadOnlyCollection<AdmissionDto> GetAdmissionHistory(Guid patientId)
        => db.Admissions.AsNoTracking().Where(x => x.PatientId == patientId).OrderByDescending(x => x.RequestedAtUtc).Select(x => MapAdmission(x)).ToArray();

    public IReadOnlyCollection<BedMovementDto> GetBedHistory(Guid admissionId)
        => db.BedMovements.AsNoTracking()
            .Where(x => x.AdmissionId == admissionId)
            .OrderByDescending(x => x.MovedAtUtc)
            .Select(x => new BedMovementDto(x.Id, x.AdmissionId, x.FromBedId, x.ToBedId, x.MovedAtUtc, x.Reason))
            .ToArray();

    public IReadOnlyCollection<BedBoardItemDto> GetBedBoard(Guid? floorId)
    {
        var query = db.Beds
            .AsNoTracking()
            .Include(x => x.Room)
            .ThenInclude(x => x.Ward)
            .ThenInclude(x => x.Floor)
            .ThenInclude(x => x.Building)
            .AsQueryable();

        if (floorId is not null)
        {
            query = query.Where(x => x.Room.Ward.FloorId == floorId);
        }

        var activeAdmissions = db.Admissions
            .AsNoTracking()
            .Where(x => x.Status == "Admitted" && x.CurrentBedId != null)
            .ToDictionary(x => x.CurrentBedId!.Value, x => x);

        return query
            .OrderBy(x => x.Room.Ward.Floor.Building.Name)
            .ThenBy(x => x.Room.Ward.Floor.Name)
            .ThenBy(x => x.Room.Name)
            .ThenBy(x => x.BedNumber)
            .Select(bed => new BedBoardItemDto(
                bed.Id,
                bed.Room.Ward.Floor.Building.Name,
                bed.Room.Ward.Floor.Name,
                bed.Room.Ward.Name,
                bed.Room.Name,
                bed.BedNumber,
                bed.Status,
                activeAdmissions.ContainsKey(bed.Id) ? activeAdmissions[bed.Id].Id : null,
                activeAdmissions.ContainsKey(bed.Id) ? activeAdmissions[bed.Id].PatientName : null))
            .ToArray();
    }

    private static BedDto MapBed(BedEntity b) => new(b.Id, b.RoomId, b.BedNumber, b.Status);

    private static AdmissionDto MapAdmission(AdmissionEntity a)
        => new(a.Id, a.PatientId, a.PatientName, a.Source, a.RequestedAtUtc, a.AdmittedAtUtc, a.DepositAmount, a.DepositReference, a.CurrentBedId, a.Status);
}
