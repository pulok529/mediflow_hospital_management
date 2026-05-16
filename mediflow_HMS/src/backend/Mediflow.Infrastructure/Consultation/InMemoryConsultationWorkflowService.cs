using Mediflow.Application.Abstractions.Audit;
using Mediflow.Application.Abstractions.Consultation;

namespace Mediflow.Infrastructure.Consultation;

internal sealed class InMemoryConsultationWorkflowService(IAuditLogService auditLogService) : IConsultationWorkflowService
{
    private static readonly object Sync = new();
    private static readonly List<EncounterRecord> Encounters = [];

    public EncounterDto StartEncounter(StartEncounterRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var existingOpen = Encounters.FirstOrDefault(x => x.AppointmentId == request.AppointmentId && x.State == EncounterState.Open);
            if (existingOpen is not null) return Map(existingOpen);

            var encounter = new EncounterRecord
            {
                Id = Guid.NewGuid(),
                AppointmentId = request.AppointmentId,
                PatientId = request.PatientId,
                PatientName = request.PatientName,
                StartedAtUtc = DateTime.UtcNow,
                State = EncounterState.Open,
                Vitals = new VitalDto(null, null, null, null, null, null, null, null),
                ChiefComplaint = string.Empty,
                Diagnosis = string.Empty,
                ClinicalNotes = string.Empty,
                Prescriptions = [],
                Orders = []
            };
            Encounters.Add(encounter);
            auditLogService.Add(actorUserId, "Encounter.Start", "Encounter", encounter.Id.ToString(), $"Started encounter for {request.PatientName}");
            return Map(encounter);
        }
    }

    public EncounterDto? SaveConsultation(Guid encounterId, SaveConsultationRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var encounter = Encounters.FirstOrDefault(x => x.Id == encounterId);
            if (encounter is null) return null;

            encounter.Vitals = request.Vitals;
            encounter.ChiefComplaint = request.ChiefComplaint;
            encounter.Diagnosis = request.Diagnosis;
            encounter.ClinicalNotes = request.ClinicalNotes;
            encounter.Prescriptions = request.Prescriptions.ToList();
            encounter.Orders = request.Orders.ToList();
            encounter.FollowUpDate = request.FollowUpDate;
            encounter.FollowUpAdvice = request.FollowUpAdvice;

            if (request.CompleteEncounter)
            {
                encounter.State = EncounterState.Completed;
                encounter.CompletedAtUtc = DateTime.UtcNow;
            }

            auditLogService.Add(actorUserId, "Encounter.Save", "Encounter", encounter.Id.ToString(), request.CompleteEncounter ? "Consultation completed" : "Consultation saved");
            return Map(encounter);
        }
    }

    public IReadOnlyCollection<EncounterDto> GetWaitingPatients(DateOnly date)
    {
        lock (Sync)
        {
            return Encounters.Where(x => x.StartedAtUtc.Date == date.ToDateTime(TimeOnly.MinValue).Date && x.State == EncounterState.Open)
                .OrderBy(x => x.StartedAtUtc)
                .Select(Map)
                .ToArray();
        }
    }

    public IReadOnlyCollection<EncounterDto> GetPatientHistory(Guid patientId)
    {
        lock (Sync)
        {
            return Encounters.Where(x => x.PatientId == patientId).OrderByDescending(x => x.StartedAtUtc).Select(Map).ToArray();
        }
    }

    public EncounterDto? GetEncounter(Guid encounterId)
    {
        lock (Sync)
        {
            var encounter = Encounters.FirstOrDefault(x => x.Id == encounterId);
            return encounter is null ? null : Map(encounter);
        }
    }

    private static EncounterDto Map(EncounterRecord x) => new(
        x.Id, x.AppointmentId, x.PatientId, x.PatientName, x.StartedAtUtc, x.CompletedAtUtc, x.State,
        x.Vitals, x.ChiefComplaint, x.Diagnosis, x.ClinicalNotes, x.Prescriptions, x.Orders, x.FollowUpDate, x.FollowUpAdvice);

    private sealed class EncounterRecord
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public DateTime StartedAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }
        public EncounterState State { get; set; }
        public VitalDto Vitals { get; set; } = new(null, null, null, null, null, null, null, null);
        public string ChiefComplaint { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string ClinicalNotes { get; set; } = string.Empty;
        public List<PrescriptionItemDto> Prescriptions { get; set; } = [];
        public List<DoctorOrderDto> Orders { get; set; } = [];
        public DateOnly? FollowUpDate { get; set; }
        public string FollowUpAdvice { get; set; } = string.Empty;
    }
}
