using Mediflow.Application.Abstractions.Audit;
using Mediflow.Application.Abstractions.Billing;

namespace Mediflow.Infrastructure.Billing;

internal sealed class InMemoryBillingWorkflowService(IAuditLogService auditLogService) : IBillingWorkflowService
{
    private static readonly object Sync = new();
    private static readonly List<BillDto> Bills = [];
    private static readonly List<PaymentDto> Payments = [];
    private static readonly List<RefundDto> Refunds = [];
    private static readonly List<DiscountRequestDto> Discounts = [];
    private static readonly Dictionary<Guid, DischargeSummaryDto> Discharges = [];

    public BillDto CreateOpdBill(CreateOpdBillRequest request, Guid? actorUserId) => CreateBill(null, request.EncounterId, request.PatientName, BillType.OPD, request.Lines, actorUserId);
    public BillDto CreateIpdBill(CreateIpdBillRequest request, Guid? actorUserId) => CreateBill(request.AdmissionId, null, request.PatientName, BillType.IPD, request.Lines, actorUserId);

    private BillDto CreateBill(Guid? admissionId, Guid? encounterId, string patient, BillType type, IReadOnlyCollection<BillLineDto> lines, Guid? actor)
    {
        lock (Sync)
        {
            var gross = lines.Sum(x => x.Amount);
            var bill = new BillDto(Guid.NewGuid(), admissionId, encounterId, patient, type, gross, 0, 0, 0, false, lines.ToArray());
            Bills.Add(bill);
            auditLogService.Add(actor, "Billing.Create", "Bill", bill.Id.ToString(), type.ToString());
            return bill;
        }
    }

    public BillDto? AddRunningBillLine(AddRunningBillLineRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var bill = Bills.FirstOrDefault(x => x.Id == request.BillId); if (bill is null) return null;
            var lines = bill.Lines.ToList(); lines.Add(request.Line);
            bill = bill with { Lines = lines.ToArray(), GrossAmount = lines.Sum(x => x.Amount) };
            Replace(Bills, bill, x => x.Id == bill.Id);
            auditLogService.Add(actorUserId, "Billing.Running.AddLine", "Bill", bill.Id.ToString(), request.Line.Item);
            return bill;
        }
    }

    public PaymentDto AddPayment(AddPaymentRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var bill = Bills.First(x => x.Id == request.BillId);
            var p = new PaymentDto(Guid.NewGuid(), bill.Id, request.Amount, request.Mode, DateTime.UtcNow, request.IsAdvance);
            Payments.Add(p);
            bill = bill with { PaidAmount = bill.PaidAmount + request.Amount };
            Replace(Bills, bill, x => x.Id == bill.Id);
            auditLogService.Add(actorUserId, "Billing.Payment", "Payment", p.Id.ToString(), request.Mode);
            return p;
        }
    }

    public RefundDto CreateRefund(CreateRefundRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var bill = Bills.First(x => x.Id == request.BillId);
            var r = new RefundDto(Guid.NewGuid(), bill.Id, request.Amount, request.Reason, DateTime.UtcNow);
            Refunds.Add(r);
            bill = bill with { RefundAmount = bill.RefundAmount + request.Amount, PaidAmount = bill.PaidAmount - request.Amount };
            Replace(Bills, bill, x => x.Id == bill.Id);
            auditLogService.Add(actorUserId, "Billing.Refund", "Refund", r.Id.ToString(), request.Reason);
            return r;
        }
    }

    public DiscountRequestDto RequestDiscount(RequestDiscountRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var d = new DiscountRequestDto(Guid.NewGuid(), request.BillId, request.Amount, request.Reason, DiscountStatus.Pending, DateTime.UtcNow);
            Discounts.Add(d);
            auditLogService.Add(actorUserId, "Billing.Discount.Request", "DiscountRequest", d.Id.ToString(), request.Reason);
            return d;
        }
    }

    public DiscountRequestDto? ResolveDiscount(Guid discountRequestId, ResolveDiscountRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var d = Discounts.FirstOrDefault(x => x.Id == discountRequestId); if (d is null) return null;
            d = d with { Status = request.Status }; Replace(Discounts, d, x => x.Id == d.Id);
            if (request.Status == DiscountStatus.Approved)
            {
                var bill = Bills.First(x => x.Id == d.BillId);
                bill = bill with { DiscountAmount = bill.DiscountAmount + d.Amount };
                Replace(Bills, bill, x => x.Id == bill.Id);
            }
            auditLogService.Add(actorUserId, "Billing.Discount.Resolve", "DiscountRequest", d.Id.ToString(), request.Status.ToString());
            return d;
        }
    }

    public BillDto? FinalClearance(FinalClearanceRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var bill = Bills.FirstOrDefault(x => x.Id == request.BillId); if (bill is null) return null;
            var net = bill.GrossAmount - bill.DiscountAmount;
            if (bill.PaidAmount < net) throw new InvalidOperationException("Outstanding amount exists.");
            bill = bill with { FinalCleared = true }; Replace(Bills, bill, x => x.Id == bill.Id);
            auditLogService.Add(actorUserId, "Billing.FinalClearance", "Bill", bill.Id.ToString(), "Cleared");
            return bill;
        }
    }

    public DischargeSummaryDto SaveDischargeSummary(SaveDischargeSummaryRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var billCleared = Bills.Where(b => b.AdmissionId == request.AdmissionId).All(b => b.FinalCleared);
            var s = new DischargeSummaryDto(request.AdmissionId, request.PatientName, request.Diagnosis, request.TreatmentSummary, request.Advice, request.DischargeDate, billCleared);
            Discharges[request.AdmissionId] = s;
            auditLogService.Add(actorUserId, "Discharge.Summary.Save", "DischargeSummary", request.AdmissionId.ToString(), request.PatientName);
            return s;
        }
    }

    public DischargeSummaryDto? GetDischargeSummary(Guid admissionId) => Discharges.TryGetValue(admissionId, out var x) ? x : null;
    public IReadOnlyCollection<BillDto> GetBills() => Bills.OrderByDescending(x => x.Id).ToArray();
    public BillDto? GetBill(Guid billId) => Bills.FirstOrDefault(x => x.Id == billId);
    public IReadOnlyCollection<DiscountRequestDto> GetDiscountQueue() => Discounts.Where(x => x.Status == DiscountStatus.Pending).OrderBy(x => x.RequestedAtUtc).ToArray();
    public IReadOnlyCollection<PaymentDto> GetPayments(Guid billId) => Payments.Where(x => x.BillId == billId).OrderByDescending(x => x.AtUtc).ToArray();
    public IReadOnlyCollection<RefundDto> GetRefunds(Guid billId) => Refunds.Where(x => x.BillId == billId).OrderByDescending(x => x.AtUtc).ToArray();

    private static void Replace<T>(List<T> list, T updated, Func<T, bool> predicate) { var i = list.FindIndex(x => predicate(x)); if (i >= 0) list[i] = updated; }
}
