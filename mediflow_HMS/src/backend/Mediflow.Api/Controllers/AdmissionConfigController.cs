using ApiCreateBedRequest = Mediflow.Api.Contracts.Admission.CreateBedRequest;
using ApiCreateBuildingRequest = Mediflow.Api.Contracts.Admission.CreateBuildingRequest;
using ApiCreateFloorRequest = Mediflow.Api.Contracts.Admission.CreateFloorRequest;
using ApiCreateRoomRequest = Mediflow.Api.Contracts.Admission.CreateRoomRequest;
using ApiCreateWardRequest = Mediflow.Api.Contracts.Admission.CreateWardRequest;
using ApiUpdateBedStatusRequest = Mediflow.Api.Contracts.Admission.UpdateBedStatusRequest;
using Mediflow.Application.Abstractions.Admission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mediflow.Api.Controllers;

[ApiController]
[Route("api/admission/config")]
[Authorize(Roles = "SuperAdmin,Admin,AdmissionOfficer")]
public sealed class AdmissionConfigController(IAdmissionWorkflowService service) : ControllerBase
{
    [HttpPost("buildings")]
    public IActionResult CreateBuilding([FromBody] ApiCreateBuildingRequest request)
        => Ok(service.CreateBuilding(new(request.Name), ParseActorId(User)));

    [HttpPost("floors")]
    public IActionResult CreateFloor([FromBody] ApiCreateFloorRequest request)
        => Ok(service.CreateFloor(new(request.BuildingId, request.Name), ParseActorId(User)));

    [HttpPost("wards")]
    public IActionResult CreateWard([FromBody] ApiCreateWardRequest request)
        => Ok(service.CreateWard(new(request.FloorId, request.Name), ParseActorId(User)));

    [HttpPost("rooms")]
    public IActionResult CreateRoom([FromBody] ApiCreateRoomRequest request)
        => Ok(service.CreateRoom(new(request.WardId, request.Name), ParseActorId(User)));

    [HttpPost("beds")]
    public IActionResult CreateBed([FromBody] ApiCreateBedRequest request)
        => Ok(service.CreateBed(new(request.RoomId, request.BedNumber), ParseActorId(User)));

    [HttpPut("beds/{bedId:guid}/status")]
    public IActionResult UpdateBedStatus(Guid bedId, [FromBody] ApiUpdateBedStatusRequest request)
    {
        var updated = service.UpdateBedStatus(bedId, new(request.Status), ParseActorId(User));
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpGet("hierarchy")]
    public IActionResult Hierarchy() => Ok(new
    {
        buildings = service.GetBuildings(),
        floors = service.GetFloors(),
        wards = service.GetWards(),
        rooms = service.GetRooms(),
        beds = service.GetBeds()
    });

    private static Guid? ParseActorId(ClaimsPrincipal principal)
        => Guid.TryParse(principal.FindFirstValue("sub"), out var id) ? id : null;
}
