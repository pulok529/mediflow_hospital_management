using Mediflow.Application.Abstractions.Admission;
using Mediflow.Application.Abstractions.Audit;

namespace Mediflow.Infrastructure.Admission;

internal sealed class InMemoryAdmissionWorkflowService(IAuditLogService auditLogService) : IAdmissionWorkflowService
{
    private static readonly object Sync = new();
    private static readonly List<BuildingRecord> Buildings = [];
    private static readonly List<FloorRecord> Floors = [];
    private static readonly List<WardRecord> Wards = [];
    private static readonly List<RoomRecord> Rooms = [];
    private static readonly List<BedRecord> Beds = [];
    private static readonly List<AdmissionRecord> Admissions = [];
    private static readonly List<BedMovementRecord> BedMovements = [];

    public BuildingDto CreateBuilding(CreateBuildingRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var entity = new BuildingRecord { Id = Guid.NewGuid(), Name = request.Name };
            Buildings.Add(entity);
            auditLogService.Add(actorUserId, "Building.Create", "Building", entity.Id.ToString(), entity.Name);
            return new BuildingDto(entity.Id, entity.Name);
        }
    }

    public FloorDto CreateFloor(CreateFloorRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var entity = new FloorRecord { Id = Guid.NewGuid(), BuildingId = request.BuildingId, Name = request.Name };
            Floors.Add(entity);
            auditLogService.Add(actorUserId, "Floor.Create", "Floor", entity.Id.ToString(), entity.Name);
            return new FloorDto(entity.Id, entity.BuildingId, entity.Name);
        }
    }

    public WardDto CreateWard(CreateWardRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var entity = new WardRecord { Id = Guid.NewGuid(), FloorId = request.FloorId, Name = request.Name };
            Wards.Add(entity);
            auditLogService.Add(actorUserId, "Ward.Create", "Ward", entity.Id.ToString(), entity.Name);
            return new WardDto(entity.Id, entity.FloorId, entity.Name);
        }
    }

    public RoomDto CreateRoom(CreateRoomRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var entity = new RoomRecord { Id = Guid.NewGuid(), WardId = request.WardId, Name = request.Name };
            Rooms.Add(entity);
            auditLogService.Add(actorUserId, "Room.Create", "Room", entity.Id.ToString(), entity.Name);
            return new RoomDto(entity.Id, entity.WardId, entity.Name);
        }
    }

    public BedDto CreateBed(CreateBedRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var entity = new BedRecord { Id = Guid.NewGuid(), RoomId = request.RoomId, BedNumber = request.BedNumber, Status = BedStatus.Available };
            Beds.Add(entity);
            auditLogService.Add(actorUserId, "Bed.Create", "Bed", entity.Id.ToString(), entity.BedNumber);
            return new BedDto(entity.Id, entity.RoomId, entity.BedNumber, entity.Status);
        }
    }

    public BedDto? UpdateBedStatus(Guid bedId, UpdateBedStatusRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var bed = Beds.FirstOrDefault(x => x.Id == bedId);
            if (bed is null) return null;
            bed.Status = request.Status;
            auditLogService.Add(actorUserId, "Bed.Status", "Bed", bed.Id.ToString(), bed.Status.ToString());
            return new BedDto(bed.Id, bed.RoomId, bed.BedNumber, bed.Status);
        }
    }

    public IReadOnlyCollection<BuildingDto> GetBuildings() => Buildings.Select(x => new BuildingDto(x.Id, x.Name)).ToArray();
    public IReadOnlyCollection<FloorDto> GetFloors() => Floors.Select(x => new FloorDto(x.Id, x.BuildingId, x.Name)).ToArray();
    public IReadOnlyCollection<WardDto> GetWards() => Wards.Select(x => new WardDto(x.Id, x.FloorId, x.Name)).ToArray();
    public IReadOnlyCollection<RoomDto> GetRooms() => Rooms.Select(x => new RoomDto(x.Id, x.WardId, x.Name)).ToArray();
    public IReadOnlyCollection<BedDto> GetBeds() => Beds.Select(x => new BedDto(x.Id, x.RoomId, x.BedNumber, x.Status)).ToArray();

    public AdmissionDto CreateAdmissionRequest(CreateAdmissionRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var admission = new AdmissionRecord
            {
                Id = Guid.NewGuid(),
                PatientId = request.PatientId,
                PatientName = request.PatientName,
                Source = request.Source,
                RequestedAtUtc = DateTime.UtcNow,
                DepositAmount = request.DepositAmount,
                DepositReference = request.DepositReference,
                Status = "Requested"
            };
            Admissions.Add(admission);
            auditLogService.Add(actorUserId, "Admission.Request", "Admission", admission.Id.ToString(), $"Source={request.Source}");
            return MapAdmission(admission);
        }
    }

    public AdmissionDto? AssignBed(Guid admissionId, AssignBedRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var admission = Admissions.FirstOrDefault(x => x.Id == admissionId);
            var bed = Beds.FirstOrDefault(x => x.Id == request.BedId);
            if (admission is null || bed is null) return null;
            if (bed.Status != BedStatus.Available && bed.Status != BedStatus.Reserved)
                throw new InvalidOperationException("Bed is not assignable.");

            bed.Status = BedStatus.Occupied;
            admission.CurrentBedId = bed.Id;
            admission.AdmittedAtUtc = DateTime.UtcNow;
            admission.Status = "Admitted";

            BedMovements.Add(new BedMovementRecord
            {
                Id = Guid.NewGuid(),
                AdmissionId = admission.Id,
                FromBedId = null,
                ToBedId = bed.Id,
                MovedAtUtc = DateTime.UtcNow,
                Reason = "Initial assignment"
            });

            auditLogService.Add(actorUserId, "Bed.Assign", "Admission", admission.Id.ToString(), $"Bed={bed.BedNumber}");
            auditLogService.Add(actorUserId, "Bed.Movement", "BedMovement", BedMovements.Last().Id.ToString(), "Initial assignment");
            return MapAdmission(admission);
        }
    }

    public AdmissionDto? TransferBed(Guid admissionId, TransferBedRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var admission = Admissions.FirstOrDefault(x => x.Id == admissionId);
            var toBed = Beds.FirstOrDefault(x => x.Id == request.ToBedId);
            if (admission is null || toBed is null || admission.CurrentBedId is null) return null;

            var fromBed = Beds.First(x => x.Id == admission.CurrentBedId.Value);
            if (toBed.Status != BedStatus.Available && toBed.Status != BedStatus.Reserved)
                throw new InvalidOperationException("Target bed is not assignable.");

            fromBed.Status = BedStatus.TransferPending;
            toBed.Status = BedStatus.Occupied;
            admission.CurrentBedId = toBed.Id;

            BedMovements.Add(new BedMovementRecord
            {
                Id = Guid.NewGuid(),
                AdmissionId = admission.Id,
                FromBedId = fromBed.Id,
                ToBedId = toBed.Id,
                MovedAtUtc = DateTime.UtcNow,
                Reason = request.Reason
            });

            fromBed.Status = BedStatus.CleaningRequired;

            auditLogService.Add(actorUserId, "Bed.Transfer", "Admission", admission.Id.ToString(), $"{fromBed.BedNumber}->{toBed.BedNumber}");
            auditLogService.Add(actorUserId, "Bed.Movement", "BedMovement", BedMovements.Last().Id.ToString(), request.Reason);
            return MapAdmission(admission);
        }
    }

    public IReadOnlyCollection<AdmissionDto> GetAdmissions() => Admissions.OrderByDescending(x => x.RequestedAtUtc).Select(MapAdmission).ToArray();
    public IReadOnlyCollection<AdmissionDto> GetAdmissionHistory(Guid patientId) => Admissions.Where(x => x.PatientId == patientId).OrderByDescending(x => x.RequestedAtUtc).Select(MapAdmission).ToArray();

    public IReadOnlyCollection<BedMovementDto> GetBedHistory(Guid admissionId)
        => BedMovements.Where(x => x.AdmissionId == admissionId).OrderByDescending(x => x.MovedAtUtc)
            .Select(x => new BedMovementDto(x.Id, x.AdmissionId, x.FromBedId, x.ToBedId, x.MovedAtUtc, x.Reason)).ToArray();

    public IReadOnlyCollection<BedBoardItemDto> GetBedBoard(Guid? floorId)
    {
        lock (Sync)
        {
            var board = new List<BedBoardItemDto>();
            foreach (var bed in Beds)
            {
                var room = Rooms.FirstOrDefault(x => x.Id == bed.RoomId);
                if (room is null) continue;
                var ward = Wards.FirstOrDefault(x => x.Id == room.WardId);
                if (ward is null) continue;
                var floor = Floors.FirstOrDefault(x => x.Id == ward.FloorId);
                if (floor is null) continue;
                if (floorId is not null && floor.Id != floorId) continue;
                var building = Buildings.FirstOrDefault(x => x.Id == floor.BuildingId);
                if (building is null) continue;

                var admission = Admissions.FirstOrDefault(a => a.CurrentBedId == bed.Id && a.Status == "Admitted");
                board.Add(new BedBoardItemDto(
                    bed.Id,
                    building.Name,
                    floor.Name,
                    ward.Name,
                    room.Name,
                    bed.BedNumber,
                    bed.Status,
                    admission?.Id,
                    admission?.PatientName));
            }

            return board.OrderBy(x => x.Building).ThenBy(x => x.Floor).ThenBy(x => x.Room).ThenBy(x => x.BedNumber).ToArray();
        }
    }

    private static AdmissionDto MapAdmission(AdmissionRecord a)
        => new(a.Id, a.PatientId, a.PatientName, a.Source, a.RequestedAtUtc, a.AdmittedAtUtc, a.DepositAmount, a.DepositReference, a.CurrentBedId, a.Status);

    private sealed class BuildingRecord { public Guid Id { get; set; } public string Name { get; set; } = string.Empty; }
    private sealed class FloorRecord { public Guid Id { get; set; } public Guid BuildingId { get; set; } public string Name { get; set; } = string.Empty; }
    private sealed class WardRecord { public Guid Id { get; set; } public Guid FloorId { get; set; } public string Name { get; set; } = string.Empty; }
    private sealed class RoomRecord { public Guid Id { get; set; } public Guid WardId { get; set; } public string Name { get; set; } = string.Empty; }
    private sealed class BedRecord { public Guid Id { get; set; } public Guid RoomId { get; set; } public string BedNumber { get; set; } = string.Empty; public BedStatus Status { get; set; } }
    private sealed class AdmissionRecord
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public DateTime RequestedAtUtc { get; set; }
        public DateTime? AdmittedAtUtc { get; set; }
        public decimal DepositAmount { get; set; }
        public string? DepositReference { get; set; }
        public Guid? CurrentBedId { get; set; }
        public string Status { get; set; } = string.Empty;
    }
    private sealed class BedMovementRecord
    {
        public Guid Id { get; set; }
        public Guid AdmissionId { get; set; }
        public Guid? FromBedId { get; set; }
        public Guid ToBedId { get; set; }
        public DateTime MovedAtUtc { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
