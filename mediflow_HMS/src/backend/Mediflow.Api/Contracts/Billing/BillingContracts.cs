using Mediflow.Application.Abstractions.Billing;

namespace Mediflow.Api.Contracts.Billing;

public sealed record CreateOpdBillRequest(Guid? EncounterId, string PatientName, IReadOnlyCollection<BillLineDto> Lines);
public sealed record CreateIpdBillRequest(Guid AdmissionId, string PatientName, IReadOnlyCollection<BillLineDto> Lines);
public sealed record AddRunningBillLineRequest(Guid BillId, BillLineDto Line);
public sealed record AddPaymentRequest(Guid BillId, decimal Amount, string Mode, bool IsAdvance);
public sealed record RequestDiscountRequest(Guid BillId, decimal Amount, string Reason);
public sealed record ResolveDiscountRequest(DiscountStatus Status);
public sealed record CreateRefundRequest(Guid BillId, decimal Amount, string Reason);
public sealed record FinalClearanceRequest(Guid BillId);
public sealed record SaveDischargeSummaryRequest(Guid AdmissionId, string PatientName, string Diagnosis, string TreatmentSummary, string Advice, DateOnly DischargeDate);
