using Mediflow.Application.Abstractions.Nursing;

namespace Mediflow.Api.Contracts.Nursing;

public sealed record CreateNursingNoteRequest(Guid AdmissionId, string NurseName, string Note, Guid? LinkedDoctorRoundEncounterId);
public sealed record AddMedicationRequest(Guid AdmissionId, string MedicationName, string Dose, DateTime DueAtUtc);
public sealed record AdministerMedicationRequest(string? Remarks);
public sealed record ScheduleVitalsRequest(Guid AdmissionId, DateTime DueAtUtc);
public sealed record RecordVitalsRequest(decimal? TemperatureC, int? Pulse, int? SystolicBp, int? DiastolicBp, decimal? SpO2);
public sealed record AddIntakeOutputRequest(Guid AdmissionId, decimal IntakeMl, decimal OutputMl, string Notes);
public sealed record AddShiftHandoverRequest(Guid AdmissionId, string FromShift, string ToShift, string Summary);
