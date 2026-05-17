using Mediflow.Application.Abstractions.Audit;
using Mediflow.Application.Abstractions.Consultation;
using Mediflow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mediflow.Infrastructure.Consultation;

internal sealed class DatabaseConsultationWorkflowService(AppDbContext db, IAuditLogService auditLogService) : IConsultationWorkflowService
{
    public EncounterDto StartEncounter(StartEncounterRequest request, Guid? actorUserId)
    {
        var existingOpen = QueryEncounters()
            .FirstOrDefault(x => x.AppointmentId == request.AppointmentId && x.State == EncounterState.Open);
        if (existingOpen is not null) return Map(existingOpen);

        var encounter = new EncounterEntity
        {
            AppointmentId = request.AppointmentId,
            PatientId = request.PatientId,
            PatientName = request.PatientName,
            StartedAtUtc = DateTime.UtcNow,
            State = EncounterState.Open,
            CreatedBy = actorUserId,
            Vitals = new EncounterVitalEntity()
        };

        db.Encounters.Add(encounter);
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Encounter.Start", "Encounter", encounter.Id.ToString(), $"Started encounter for {request.PatientName}");
        return Map(QueryEncounters().First(x => x.Id == encounter.Id));
    }

    public EncounterDto? SaveConsultation(Guid encounterId, SaveConsultationRequest request, Guid? actorUserId)
    {
        var encounter = db.Encounters
            .Include(x => x.Vitals)
            .Include(x => x.Prescriptions)
            .Include(x => x.Orders)
            .FirstOrDefault(x => x.Id == encounterId);

        if (encounter is null) return null;
        if (encounter.State == EncounterState.Completed)
            throw new InvalidOperationException("Completed encounters are locked. Create a revision workflow before changing this record.");

        if (request.CompleteEncounter && string.IsNullOrWhiteSpace(request.Diagnosis) && string.IsNullOrWhiteSpace(request.ClinicalNotes))
            throw new InvalidOperationException("Diagnosis or clinical notes are required before completing consultation.");

        encounter.ChiefComplaint = request.ChiefComplaint;
        encounter.Diagnosis = request.Diagnosis;
        encounter.ClinicalNotes = request.ClinicalNotes;
        encounter.FollowUpDate = request.FollowUpDate;
        encounter.FollowUpAdvice = request.FollowUpAdvice;
        encounter.UpdatedAtUtc = DateTime.UtcNow;
        encounter.UpdatedBy = actorUserId;

        encounter.Vitals ??= new EncounterVitalEntity { EncounterId = encounter.Id };
        encounter.Vitals.TemperatureC = request.Vitals.TemperatureC;
        encounter.Vitals.Pulse = request.Vitals.Pulse;
        encounter.Vitals.SystolicBp = request.Vitals.SystolicBp;
        encounter.Vitals.DiastolicBp = request.Vitals.DiastolicBp;
        encounter.Vitals.RespiratoryRate = request.Vitals.RespiratoryRate;
        encounter.Vitals.SpO2 = request.Vitals.SpO2;
        encounter.Vitals.WeightKg = request.Vitals.WeightKg;
        encounter.Vitals.HeightCm = request.Vitals.HeightCm;

        db.PrescriptionItems.RemoveRange(encounter.Prescriptions);
        db.DoctorOrders.RemoveRange(encounter.Orders);
        encounter.Prescriptions.Clear();
        encounter.Orders.Clear();

        var prescriptions = request.Prescriptions.Select(x => new PrescriptionItemEntity
        {
            EncounterId = encounter.Id,
            MedicineName = x.MedicineName,
            Dose = x.Dose,
            Frequency = x.Frequency,
            Duration = x.Duration,
            Instructions = x.Instructions
        }).ToList();
        var orders = request.Orders.Select(x => new DoctorOrderEntity
        {
            EncounterId = encounter.Id,
            Type = x.Type,
            TestOrProcedure = x.TestOrProcedure,
            Notes = x.Notes
        }).ToList();
        db.PrescriptionItems.AddRange(prescriptions);
        db.DoctorOrders.AddRange(orders);

        if (request.CompleteEncounter)
        {
            encounter.State = EncounterState.Completed;
            encounter.CompletedAtUtc = DateTime.UtcNow;
        }

        db.SaveChanges();
        auditLogService.Add(actorUserId, "Encounter.Save", "Encounter", encounter.Id.ToString(), request.CompleteEncounter ? "Consultation completed" : "Consultation saved");
        return Map(QueryEncounters().First(x => x.Id == encounter.Id));
    }

    public IReadOnlyCollection<EncounterDto> GetWaitingPatients(DateOnly date)
        => QueryEncounters()
            .Where(x => x.StartedAtUtc.Date == date.ToDateTime(TimeOnly.MinValue).Date && x.State == EncounterState.Open)
            .OrderBy(x => x.StartedAtUtc)
            .Select(Map)
            .ToArray();

    public IReadOnlyCollection<EncounterDto> GetPatientHistory(Guid patientId)
        => QueryEncounters()
            .Where(x => x.PatientId == patientId)
            .OrderByDescending(x => x.StartedAtUtc)
            .Select(Map)
            .ToArray();

    public EncounterDto? GetEncounter(Guid encounterId)
    {
        var encounter = QueryEncounters().FirstOrDefault(x => x.Id == encounterId);
        return encounter is null ? null : Map(encounter);
    }

    private IQueryable<EncounterEntity> QueryEncounters()
        => db.Encounters
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.Vitals)
            .Include(x => x.Prescriptions)
            .Include(x => x.Orders);

    private static EncounterDto Map(EncounterEntity x)
    {
        var vitals = x.Vitals is null
            ? new VitalDto(null, null, null, null, null, null, null, null)
            : new VitalDto(x.Vitals.TemperatureC, x.Vitals.Pulse, x.Vitals.SystolicBp, x.Vitals.DiastolicBp, x.Vitals.RespiratoryRate, x.Vitals.SpO2, x.Vitals.WeightKg, x.Vitals.HeightCm);

        return new EncounterDto(
            x.Id,
            x.AppointmentId,
            x.PatientId,
            x.PatientName,
            x.StartedAtUtc,
            x.CompletedAtUtc,
            x.State,
            vitals,
            x.ChiefComplaint,
            x.Diagnosis,
            x.ClinicalNotes,
            x.Prescriptions.Select(p => new PrescriptionItemDto(p.MedicineName, p.Dose, p.Frequency, p.Duration, p.Instructions)).ToArray(),
            x.Orders.Select(o => new DoctorOrderDto(o.Type, o.TestOrProcedure, o.Notes)).ToArray(),
            x.FollowUpDate,
            x.FollowUpAdvice);
    }
}
