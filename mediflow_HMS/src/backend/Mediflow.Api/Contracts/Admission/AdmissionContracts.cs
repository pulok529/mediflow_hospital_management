using Mediflow.Application.Abstractions.Admission;

namespace Mediflow.Api.Contracts.Admission;

public sealed record CreateBuildingRequest(string Name);
public sealed record CreateFloorRequest(Guid BuildingId, string Name);
public sealed record CreateWardRequest(Guid FloorId, string Name);
public sealed record CreateRoomRequest(Guid WardId, string Name);
public sealed record CreateBedRequest(Guid RoomId, string BedNumber);
public sealed record UpdateBedStatusRequest(BedStatus Status);

public sealed record CreateAdmissionRequest(Guid PatientId, string PatientName, string Source, decimal DepositAmount, string? DepositReference);
public sealed record AssignBedRequest(Guid BedId);
public sealed record TransferBedRequest(Guid ToBedId, string Reason);
