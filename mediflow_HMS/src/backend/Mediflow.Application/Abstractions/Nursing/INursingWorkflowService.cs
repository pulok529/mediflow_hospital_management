namespace Mediflow.Application.Abstractions.Nursing;

public enum MedicationStatus { Due, Administered, Missed }
public enum AlertSeverity { Info, Warning, Critical }

public sealed record NursingNoteDto(Guid Id, Guid AdmissionId, DateTime AtUtc, string NurseName, string Note, Guid? LinkedDoctorRoundEncounterId);
public sealed record MedicationAdministrationDto(Guid Id, Guid AdmissionId, string MedicationName, string Dose, DateTime DueAtUtc, DateTime? AdministeredAtUtc, MedicationStatus Status, string? Remarks);
public sealed record VitalsScheduleDto(Guid Id, Guid AdmissionId, DateTime DueAtUtc, decimal? TemperatureC, int? Pulse, int? SystolicBp, int? DiastolicBp, decimal? SpO2, bool Recorded);
public sealed record IntakeOutputDto(Guid Id, Guid AdmissionId, DateTime AtUtc, decimal IntakeMl, decimal OutputMl, string Notes);
public sealed record ShiftHandoverDto(Guid Id, Guid AdmissionId, DateTime AtUtc, string FromShift, string ToShift, string Summary);
public sealed record CareAlertDto(Guid Id, Guid AdmissionId, AlertSeverity Severity, string Message, DateTime AtUtc, bool Resolved);

public sealed record CreateNursingNoteRequest(Guid AdmissionId, string NurseName, string Note, Guid? LinkedDoctorRoundEncounterId);
public sealed record AddMedicationRequest(Guid AdmissionId, string MedicationName, string Dose, DateTime DueAtUtc);
public sealed record AdministerMedicationRequest(string? Remarks);
public sealed record ScheduleVitalsRequest(Guid AdmissionId, DateTime DueAtUtc);
public sealed record RecordVitalsRequest(decimal? TemperatureC, int? Pulse, int? SystolicBp, int? DiastolicBp, decimal? SpO2);
public sealed record AddIntakeOutputRequest(Guid AdmissionId, decimal IntakeMl, decimal OutputMl, string Notes);
public sealed record AddShiftHandoverRequest(Guid AdmissionId, string FromShift, string ToShift, string Summary);

public interface INursingWorkflowService
{
    NursingNoteDto AddNursingNote(CreateNursingNoteRequest request, Guid? actorUserId);
    IReadOnlyCollection<NursingNoteDto> GetNursingNotes(Guid admissionId);

    MedicationAdministrationDto AddMedication(AddMedicationRequest request, Guid? actorUserId);
    MedicationAdministrationDto? AdministerMedication(Guid medicationId, AdministerMedicationRequest request, Guid? actorUserId);
    IReadOnlyCollection<MedicationAdministrationDto> GetMedicationDueList(DateTime asOfUtc);

    VitalsScheduleDto ScheduleVitals(ScheduleVitalsRequest request, Guid? actorUserId);
    VitalsScheduleDto? RecordVitals(Guid vitalsScheduleId, RecordVitalsRequest request, Guid? actorUserId);
    IReadOnlyCollection<VitalsScheduleDto> GetVitalsSchedules(Guid admissionId);

    IntakeOutputDto AddIntakeOutput(AddIntakeOutputRequest request, Guid? actorUserId);
    IReadOnlyCollection<IntakeOutputDto> GetIntakeOutput(Guid admissionId);

    ShiftHandoverDto AddShiftHandover(AddShiftHandoverRequest request, Guid? actorUserId);
    IReadOnlyCollection<ShiftHandoverDto> GetShiftHandovers(Guid admissionId);

    IReadOnlyCollection<CareAlertDto> GetActiveAlerts();
    IReadOnlyCollection<object> GetAssignedPatients();
    IReadOnlyCollection<object> GetTransferDischargePreparation();
}
