namespace Mediflow.Application.Abstractions.Billing;

public enum BillType { OPD, IPD }
public enum DiscountStatus { Pending, Approved, Rejected }

public sealed record BillLineDto(string Item, decimal Amount);
public sealed record PaymentDto(Guid Id, Guid BillId, decimal Amount, string Mode, DateTime AtUtc, bool IsAdvance);
public sealed record RefundDto(Guid Id, Guid BillId, decimal Amount, string Reason, DateTime AtUtc);
public sealed record DiscountRequestDto(Guid Id, Guid BillId, decimal Amount, string Reason, DiscountStatus Status, DateTime RequestedAtUtc);

public sealed record BillDto(Guid Id, Guid? AdmissionId, Guid? EncounterId, string PatientName, BillType Type, decimal GrossAmount, decimal DiscountAmount, decimal PaidAmount, decimal RefundAmount, bool FinalCleared, IReadOnlyCollection<BillLineDto> Lines);
public sealed record DischargeSummaryDto(Guid AdmissionId, string PatientName, string Diagnosis, string TreatmentSummary, string Advice, DateOnly DischargeDate, bool BillingCleared);

public sealed record CreateOpdBillRequest(Guid? EncounterId, string PatientName, IReadOnlyCollection<BillLineDto> Lines);
public sealed record CreateIpdBillRequest(Guid AdmissionId, string PatientName, IReadOnlyCollection<BillLineDto> Lines);
public sealed record AddRunningBillLineRequest(Guid BillId, BillLineDto Line);
public sealed record AddPaymentRequest(Guid BillId, decimal Amount, string Mode, bool IsAdvance);
public sealed record RequestDiscountRequest(Guid BillId, decimal Amount, string Reason);
public sealed record ResolveDiscountRequest(DiscountStatus Status);
public sealed record CreateRefundRequest(Guid BillId, decimal Amount, string Reason);
public sealed record FinalClearanceRequest(Guid BillId);
public sealed record SaveDischargeSummaryRequest(Guid AdmissionId, string PatientName, string Diagnosis, string TreatmentSummary, string Advice, DateOnly DischargeDate);

public interface IBillingWorkflowService
{
    BillDto CreateOpdBill(CreateOpdBillRequest request, Guid? actorUserId);
    BillDto CreateIpdBill(CreateIpdBillRequest request, Guid? actorUserId);
    BillDto? AddRunningBillLine(AddRunningBillLineRequest request, Guid? actorUserId);

    PaymentDto AddPayment(AddPaymentRequest request, Guid? actorUserId);
    RefundDto CreateRefund(CreateRefundRequest request, Guid? actorUserId);

    DiscountRequestDto RequestDiscount(RequestDiscountRequest request, Guid? actorUserId);
    DiscountRequestDto? ResolveDiscount(Guid discountRequestId, ResolveDiscountRequest request, Guid? actorUserId);

    BillDto? FinalClearance(FinalClearanceRequest request, Guid? actorUserId);

    DischargeSummaryDto SaveDischargeSummary(SaveDischargeSummaryRequest request, Guid? actorUserId);
    DischargeSummaryDto? GetDischargeSummary(Guid admissionId);

    IReadOnlyCollection<BillDto> GetBills();
    BillDto? GetBill(Guid billId);
    IReadOnlyCollection<DiscountRequestDto> GetDiscountQueue();
    IReadOnlyCollection<PaymentDto> GetPayments(Guid billId);
    IReadOnlyCollection<RefundDto> GetRefunds(Guid billId);
}
