using ApiAddPaymentRequest = Mediflow.Api.Contracts.Billing.AddPaymentRequest;
using ApiAddRunningBillLineRequest = Mediflow.Api.Contracts.Billing.AddRunningBillLineRequest;
using ApiCreateIpdBillRequest = Mediflow.Api.Contracts.Billing.CreateIpdBillRequest;
using ApiCreateOpdBillRequest = Mediflow.Api.Contracts.Billing.CreateOpdBillRequest;
using ApiCreateRefundRequest = Mediflow.Api.Contracts.Billing.CreateRefundRequest;
using ApiFinalClearanceRequest = Mediflow.Api.Contracts.Billing.FinalClearanceRequest;
using ApiRequestDiscountRequest = Mediflow.Api.Contracts.Billing.RequestDiscountRequest;
using ApiResolveDiscountRequest = Mediflow.Api.Contracts.Billing.ResolveDiscountRequest;
using ApiSaveDischargeSummaryRequest = Mediflow.Api.Contracts.Billing.SaveDischargeSummaryRequest;
using Mediflow.Application.Abstractions.Billing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mediflow.Api.Controllers;

[ApiController]
[Route("api/billing")]
[Authorize(Roles = "SuperAdmin,Admin,BillingOfficer")]
public sealed class BillingController(IBillingWorkflowService service) : ControllerBase
{
    [HttpPost("opd")]
    public IActionResult CreateOpd([FromBody] ApiCreateOpdBillRequest request) => Ok(service.CreateOpdBill(new(request.EncounterId, request.PatientName, request.Lines), ActorId()));

    [HttpPost("ipd")]
    public IActionResult CreateIpd([FromBody] ApiCreateIpdBillRequest request) => Ok(service.CreateIpdBill(new(request.AdmissionId, request.PatientName, request.Lines), ActorId()));

    [HttpPut("running-line")]
    public IActionResult AddRunningLine([FromBody] ApiAddRunningBillLineRequest request)
    {
        var x = service.AddRunningBillLine(new(request.BillId, request.Line), ActorId()); return x is null ? NotFound() : Ok(x);
    }

    [HttpPost("payments")]
    public IActionResult AddPayment([FromBody] ApiAddPaymentRequest request) => Ok(service.AddPayment(new(request.BillId, request.Amount, request.Mode, request.IsAdvance), ActorId()));

    [HttpPost("discount/request")]
    public IActionResult RequestDiscount([FromBody] ApiRequestDiscountRequest request) => Ok(service.RequestDiscount(new(request.BillId, request.Amount, request.Reason), ActorId()));

    [HttpPut("discount/{id:guid}/resolve")]
    public IActionResult ResolveDiscount(Guid id, [FromBody] ApiResolveDiscountRequest request)
    {
        var x = service.ResolveDiscount(id, new(request.Status), ActorId()); return x is null ? NotFound() : Ok(x);
    }

    [HttpPost("refunds")]
    public IActionResult Refund([FromBody] ApiCreateRefundRequest request) => Ok(service.CreateRefund(new(request.BillId, request.Amount, request.Reason), ActorId()));

    [HttpPost("final-clearance")]
    public IActionResult FinalClearance([FromBody] ApiFinalClearanceRequest request)
    {
        var x = service.FinalClearance(new(request.BillId), ActorId()); return x is null ? NotFound() : Ok(x);
    }

    [HttpPost("discharge-summary")]
    public IActionResult SaveDischarge([FromBody] ApiSaveDischargeSummaryRequest request)
        => Ok(service.SaveDischargeSummary(new(request.AdmissionId, request.PatientName, request.Diagnosis, request.TreatmentSummary, request.Advice, request.DischargeDate), ActorId()));

    [HttpGet("discharge-summary/{admissionId:guid}")]
    public IActionResult GetDischarge(Guid admissionId)
    {
        var x = service.GetDischargeSummary(admissionId); return x is null ? NotFound() : Ok(x);
    }

    [HttpGet("dashboard")]
    public IActionResult Dashboard() => Ok(new { bills = service.GetBills(), discounts = service.GetDiscountQueue() });

    [HttpGet("bills")]
    public IActionResult Bills() => Ok(service.GetBills());

    [HttpGet("bills/{billId:guid}")]
    public IActionResult Bill(Guid billId)
    {
        var x = service.GetBill(billId); return x is null ? NotFound() : Ok(x);
    }

    [HttpGet("discount-queue")]
    public IActionResult DiscountQueue() => Ok(service.GetDiscountQueue());

    [HttpGet("payments/{billId:guid}")]
    public IActionResult Payments(Guid billId) => Ok(service.GetPayments(billId));

    [HttpGet("refunds/{billId:guid}")]
    public IActionResult Refunds(Guid billId) => Ok(service.GetRefunds(billId));

    private Guid? ActorId() => Guid.TryParse(User.FindFirstValue("sub"), out var id) ? id : null;
}
