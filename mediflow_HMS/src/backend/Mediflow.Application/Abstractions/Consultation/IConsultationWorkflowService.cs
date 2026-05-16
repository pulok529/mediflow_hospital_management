namespace Mediflow.Application.Abstractions.Consultation;

public enum EncounterState { Open, Completed }
public enum OrderType { Lab, Radiology }

public sealed record VitalDto(decimal? TemperatureC, int? Pulse, int? SystolicBp, int? DiastolicBp, int? RespiratoryRate, decimal? SpO2, decimal? WeightKg, decimal? HeightCm);
public sealed record PrescriptionItemDto(string MedicineName, string Dose, string Frequency, string Duration, string Instructions);
public sealed record DoctorOrderDto(OrderType Type, string TestOrProcedure, string Notes);

public sealed record EncounterDto(
    Guid Id,
    Guid AppointmentId,
    Guid PatientId,
    string PatientName,
    DateTime StartedAtUtc,
    DateTime? CompletedAtUtc,
    EncounterState State,
    VitalDto Vitals,
    string ChiefComplaint,
    string Diagnosis,
    string ClinicalNotes,
    IReadOnlyCollection<PrescriptionItemDto> Prescriptions,
    IReadOnlyCollection<DoctorOrderDto> Orders,
    DateOnly? FollowUpDate,
    string FollowUpAdvice);

public sealed record StartEncounterRequest(Guid AppointmentId, Guid PatientId, string PatientName);
public sealed record SaveConsultationRequest(
    VitalDto Vitals,
    string ChiefComplaint,
    string Diagnosis,
    string ClinicalNotes,
    IReadOnlyCollection<PrescriptionItemDto> Prescriptions,
    IReadOnlyCollection<DoctorOrderDto> Orders,
    DateOnly? FollowUpDate,
    string FollowUpAdvice,
    bool CompleteEncounter);

public interface IConsultationWorkflowService
{
    EncounterDto StartEncounter(StartEncounterRequest request, Guid? actorUserId);
    EncounterDto? SaveConsultation(Guid encounterId, SaveConsultationRequest request, Guid? actorUserId);
    IReadOnlyCollection<EncounterDto> GetWaitingPatients(DateOnly date);
    IReadOnlyCollection<EncounterDto> GetPatientHistory(Guid patientId);
    EncounterDto? GetEncounter(Guid encounterId);
}
