using Mediflow.Application.Abstractions.Consultation;

namespace Mediflow.Api.Contracts.Consultation;

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
