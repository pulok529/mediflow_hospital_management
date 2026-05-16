namespace Mediflow.Application.Abstractions.Admission;

public enum BedStatus
{
    Available,
    Occupied,
    Reserved,
    CleaningRequired,
    UnderCleaning,
    Maintenance,
    Isolation,
    TransferPending,
    DischargePending
}

public sealed record BuildingDto(Guid Id, string Name);
public sealed record FloorDto(Guid Id, Guid BuildingId, string Name);
public sealed record WardDto(Guid Id, Guid FloorId, string Name);
public sealed record RoomDto(Guid Id, Guid WardId, string Name);
public sealed record BedDto(Guid Id, Guid RoomId, string BedNumber, BedStatus Status);

public sealed record AdmissionDto(
    Guid Id,
    Guid PatientId,
    string PatientName,
    string Source,
    DateTime RequestedAtUtc,
    DateTime? AdmittedAtUtc,
    decimal DepositAmount,
    string? DepositReference,
    Guid? CurrentBedId,
    string Status);

public sealed record BedBoardItemDto(Guid BedId, string Building, string Floor, string Ward, string Room, string BedNumber, BedStatus Status, Guid? AdmissionId, string? PatientName);
public sealed record BedMovementDto(Guid Id, Guid AdmissionId, Guid? FromBedId, Guid ToBedId, DateTime MovedAtUtc, string Reason);

public sealed record CreateBuildingRequest(string Name);
public sealed record CreateFloorRequest(Guid BuildingId, string Name);
public sealed record CreateWardRequest(Guid FloorId, string Name);
public sealed record CreateRoomRequest(Guid WardId, string Name);
public sealed record CreateBedRequest(Guid RoomId, string BedNumber);
public sealed record UpdateBedStatusRequest(BedStatus Status);

public sealed record CreateAdmissionRequest(Guid PatientId, string PatientName, string Source, decimal DepositAmount, string? DepositReference);
public sealed record AssignBedRequest(Guid BedId);
public sealed record TransferBedRequest(Guid ToBedId, string Reason);

public interface IAdmissionWorkflowService
{
    BuildingDto CreateBuilding(CreateBuildingRequest request, Guid? actorUserId);
    FloorDto CreateFloor(CreateFloorRequest request, Guid? actorUserId);
    WardDto CreateWard(CreateWardRequest request, Guid? actorUserId);
    RoomDto CreateRoom(CreateRoomRequest request, Guid? actorUserId);
    BedDto CreateBed(CreateBedRequest request, Guid? actorUserId);
    BedDto? UpdateBedStatus(Guid bedId, UpdateBedStatusRequest request, Guid? actorUserId);

    IReadOnlyCollection<BuildingDto> GetBuildings();
    IReadOnlyCollection<FloorDto> GetFloors();
    IReadOnlyCollection<WardDto> GetWards();
    IReadOnlyCollection<RoomDto> GetRooms();
    IReadOnlyCollection<BedDto> GetBeds();

    AdmissionDto CreateAdmissionRequest(CreateAdmissionRequest request, Guid? actorUserId);
    AdmissionDto? AssignBed(Guid admissionId, AssignBedRequest request, Guid? actorUserId);
    AdmissionDto? TransferBed(Guid admissionId, TransferBedRequest request, Guid? actorUserId);

    IReadOnlyCollection<AdmissionDto> GetAdmissions();
    IReadOnlyCollection<AdmissionDto> GetAdmissionHistory(Guid patientId);
    IReadOnlyCollection<BedMovementDto> GetBedHistory(Guid admissionId);

    IReadOnlyCollection<BedBoardItemDto> GetBedBoard(Guid? floorId);
}
