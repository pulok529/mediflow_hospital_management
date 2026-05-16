namespace Mediflow.Application.Abstractions.Diagnostics;

public enum LabOrderStatus { Ordered, SampleCollected, ResultEntered, Approved }
public enum ImagingOrderStatus { Ordered, Scheduled, ReportEntered, Approved }

public sealed record LabTestCatalogDto(Guid Id, string Code, string Name, string Unit, string ReferenceRange);
public sealed record LabOrderDto(Guid Id, Guid AdmissionId, Guid? EncounterId, string PatientName, string TestCode, string TestName, LabOrderStatus Status, bool IsCritical, string? ResultValue, string? ResultNotes, DateTime OrderedAtUtc, DateTime? CollectedAtUtc, DateTime? ApprovedAtUtc);

public sealed record ImagingServiceCatalogDto(Guid Id, string Code, string Name, string Modality);
public sealed record ImagingOrderDto(Guid Id, Guid AdmissionId, Guid? EncounterId, string PatientName, string ServiceCode, string ServiceName, ImagingOrderStatus Status, DateTime OrderedAtUtc, DateTime? ScheduledAtUtc, string? ReportText, string? FileReference, DateTime? ApprovedAtUtc);

public sealed record CreateLabTestRequest(string Code, string Name, string Unit, string ReferenceRange);
public sealed record CreateLabOrderRequest(Guid AdmissionId, Guid? EncounterId, string PatientName, string TestCode);
public sealed record EnterLabResultRequest(string ResultValue, string ResultNotes, bool IsCritical);

public sealed record CreateImagingServiceRequest(string Code, string Name, string Modality);
public sealed record CreateImagingOrderRequest(Guid AdmissionId, Guid? EncounterId, string PatientName, string ServiceCode);
public sealed record ScheduleImagingRequest(DateTime ScheduledAtUtc);
public sealed record EnterImagingReportRequest(string ReportText, string FileReference);

public interface IDiagnosticsWorkflowService
{
    LabTestCatalogDto AddLabTest(CreateLabTestRequest request, Guid? actorUserId);
    IReadOnlyCollection<LabTestCatalogDto> GetLabCatalog();
    LabOrderDto CreateLabOrder(CreateLabOrderRequest request, Guid? actorUserId);
    LabOrderDto? CollectSample(Guid labOrderId, Guid? actorUserId);
    LabOrderDto? EnterLabResult(Guid labOrderId, EnterLabResultRequest request, Guid? actorUserId);
    LabOrderDto? ApproveLabResult(Guid labOrderId, Guid? actorUserId);
    IReadOnlyCollection<LabOrderDto> GetLabOrders();

    ImagingServiceCatalogDto AddImagingService(CreateImagingServiceRequest request, Guid? actorUserId);
    IReadOnlyCollection<ImagingServiceCatalogDto> GetImagingCatalog();
    ImagingOrderDto CreateImagingOrder(CreateImagingOrderRequest request, Guid? actorUserId);
    ImagingOrderDto? ScheduleImaging(Guid imagingOrderId, ScheduleImagingRequest request, Guid? actorUserId);
    ImagingOrderDto? EnterImagingReport(Guid imagingOrderId, EnterImagingReportRequest request, Guid? actorUserId);
    ImagingOrderDto? ApproveImagingReport(Guid imagingOrderId, Guid? actorUserId);
    IReadOnlyCollection<ImagingOrderDto> GetImagingOrders();
}
