namespace Mediflow.Api.Contracts.Diagnostics;

public sealed record CreateLabTestRequest(string Code, string Name, string Unit, string ReferenceRange);
public sealed record CreateLabOrderRequest(Guid AdmissionId, Guid? EncounterId, string PatientName, string TestCode);
public sealed record EnterLabResultRequest(string ResultValue, string ResultNotes, bool IsCritical);

public sealed record CreateImagingServiceRequest(string Code, string Name, string Modality);
public sealed record CreateImagingOrderRequest(Guid AdmissionId, Guid? EncounterId, string PatientName, string ServiceCode);
public sealed record ScheduleImagingRequest(DateTime ScheduledAtUtc);
public sealed record EnterImagingReportRequest(string ReportText, string FileReference);
