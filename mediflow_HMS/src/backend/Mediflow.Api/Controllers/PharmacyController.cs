using ApiAddBatchRequest = Mediflow.Api.Contracts.Pharmacy.AddBatchRequest;
using ApiCreateMedicineRequest = Mediflow.Api.Contracts.Pharmacy.CreateMedicineRequest;
using ApiCreatePurchaseOrderRequest = Mediflow.Api.Contracts.Pharmacy.CreatePurchaseOrderRequest;
using ApiCreatePurchaseRequestRequest = Mediflow.Api.Contracts.Pharmacy.CreatePurchaseRequestRequest;
using ApiCreateSupplierRequest = Mediflow.Api.Contracts.Pharmacy.CreateSupplierRequest;
using ApiGoodsReceiveRequest = Mediflow.Api.Contracts.Pharmacy.GoodsReceiveRequest;
using ApiIpdIssueRequest = Mediflow.Api.Contracts.Pharmacy.IpdIssueRequest;
using ApiIssueToDepartmentRequest = Mediflow.Api.Contracts.Pharmacy.IssueToDepartmentRequest;
using ApiOpdDispenseRequest = Mediflow.Api.Contracts.Pharmacy.OpdDispenseRequest;
using ApiReturnRequest = Mediflow.Api.Contracts.Pharmacy.ReturnRequest;
using Mediflow.Application.Abstractions.Pharmacy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mediflow.Api.Controllers;

[ApiController]
[Route("api/pharmacy")]
[Authorize(Roles = "SuperAdmin,Admin,Pharmacist")]
public sealed class PharmacyController(IPharmacyInventoryService service) : ControllerBase
{
    [HttpPost("medicines")]
    public IActionResult AddMedicine([FromBody] ApiCreateMedicineRequest request)
        => Ok(service.AddMedicine(new(request.Code, request.Name, request.Form, request.Strength, request.ReorderLevel), ActorId()));

    [HttpGet("medicines")]
    public IActionResult Medicines() => Ok(service.GetMedicines());

    [HttpPost("batches")]
    public IActionResult AddBatch([FromBody] ApiAddBatchRequest request)
        => Ok(service.AddBatch(new(request.MedicineCode, request.BatchNo, request.ExpiryDate, request.UnitCost, request.Quantity), ActorId()));

    [HttpGet("batches")]
    public IActionResult Batches([FromQuery] string? medicineCode) => Ok(service.GetBatches(medicineCode));

    [HttpPost("dispense/opd")]
    public IActionResult OpdDispense([FromBody] ApiOpdDispenseRequest request)
        => Ok(service.OpdDispense(new(request.EncounterId, request.PatientName, request.MedicineCode, request.Quantity), ActorId()));

    [HttpPost("issue/ipd")]
    public IActionResult IpdIssue([FromBody] ApiIpdIssueRequest request)
        => Ok(service.IpdIssue(new(request.AdmissionId, request.PatientName, request.MedicineCode, request.Quantity), ActorId()));

    [HttpPost("returns")]
    public IActionResult Return([FromBody] ApiReturnRequest request)
        => Ok(service.ReturnToStock(new(request.MedicineCode, request.Quantity, request.Reason), ActorId()));

    [HttpGet("low-stock-alerts")]
    public IActionResult LowStock() => Ok(service.GetLowStockAlerts());

    [HttpGet("dashboard")]
    public IActionResult Dashboard() => Ok(new
    {
        medicines = service.GetMedicines(),
        batches = service.GetBatches(),
        lowStockAlerts = service.GetLowStockAlerts(),
        dispenses = service.GetDispenses(),
        stockMovements = service.GetStockMovements()
    });

    [HttpPost("suppliers")]
    public IActionResult AddSupplier([FromBody] ApiCreateSupplierRequest request)
        => Ok(service.AddSupplier(new(request.Name, request.Contact, request.Address), ActorId()));

    [HttpGet("suppliers")]
    public IActionResult Suppliers() => Ok(service.GetSuppliers());

    [HttpPost("purchase-requests")]
    public IActionResult CreatePr([FromBody] ApiCreatePurchaseRequestRequest request)
        => Ok(service.CreatePurchaseRequest(new(request.Department), ActorId()));

    [HttpGet("purchase-requests")]
    public IActionResult Prs() => Ok(service.GetPurchaseRequests());

    [HttpPost("purchase-orders")]
    public IActionResult CreatePo([FromBody] ApiCreatePurchaseOrderRequest request)
        => Ok(service.CreatePurchaseOrder(new(request.SupplierId), ActorId()));

    [HttpGet("purchase-orders")]
    public IActionResult Pos() => Ok(service.GetPurchaseOrders());

    [HttpPost("goods-receive")]
    public IActionResult Receive([FromBody] ApiGoodsReceiveRequest request)
        => Ok(service.ReceiveGoods(new(request.PurchaseOrderId, request.Lines.Select(x => new Mediflow.Application.Abstractions.Pharmacy.ReceiveLine(x.MedicineCode, x.BatchNo, x.ExpiryDate, x.UnitCost, x.Quantity)).ToArray()), ActorId()));

    [HttpGet("goods-receive")]
    public IActionResult Grns() => Ok(service.GetGoodsReceives());

    [HttpPost("issue/department")]
    public IActionResult IssueDepartment([FromBody] ApiIssueToDepartmentRequest request)
        => Ok(service.IssueToDepartment(new(request.Department, request.MedicineCode, request.Quantity), ActorId()));

    [HttpGet("stock-movements")]
    public IActionResult Movements() => Ok(service.GetStockMovements());

    private Guid? ActorId() => Guid.TryParse(User.FindFirstValue("sub"), out var id) ? id : null;
}
