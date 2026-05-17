using Mediflow.Application.Abstractions.Admission;
using Mediflow.Application.Abstractions.Audit;
using Mediflow.Application.Abstractions.Nursing;
using Mediflow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mediflow.Infrastructure.Nursing;

internal sealed class DatabaseNursingWorkflowService(AppDbContext db, IAdmissionWorkflowService admissions, IAuditLogService auditLogService) : INursingWorkflowService
{
    public NursingNoteDto AddNursingNote(CreateNursingNoteRequest request, Guid? actorUserId)
    {
        var note = new NursingNoteEntity
        {
            AdmissionId = request.AdmissionId,
            NurseName = request.NurseName,
            Note = request.Note,
            LinkedDoctorRoundEncounterId = request.LinkedDoctorRoundEncounterId,
            CreatedBy = actorUserId
        };

        db.NursingNotes.Add(note);
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Nursing.Note", "NursingNote", note.Id.ToString(), note.Note);
        return Map(note);
    }

    public IReadOnlyCollection<NursingNoteDto> GetNursingNotes(Guid admissionId)
        => db.NursingNotes.AsNoTracking()
            .Where(x => x.AdmissionId == admissionId)
            .OrderByDescending(x => x.AtUtc)
            .Select(x => Map(x))
            .ToArray();

    public MedicationAdministrationDto AddMedication(AddMedicationRequest request, Guid? actorUserId)
    {
        var med = new MedicationAdministrationEntity
        {
            AdmissionId = request.AdmissionId,
            MedicationName = request.MedicationName,
            Dose = request.Dose,
            DueAtUtc = request.DueAtUtc,
            CreatedBy = actorUserId
        };

        db.MedicationAdministrations.Add(med);
        if (request.DueAtUtc <= DateTime.UtcNow.AddMinutes(15))
            AddAlert(request.AdmissionId, AlertSeverity.Warning, $"Medication due soon: {request.MedicationName}");

        db.SaveChanges();
        auditLogService.Add(actorUserId, "Nursing.Medication.Add", "MedicationAdministration", med.Id.ToString(), med.MedicationName);
        return Map(med);
    }

    public MedicationAdministrationDto? AdministerMedication(Guid medicationId, AdministerMedicationRequest request, Guid? actorUserId)
    {
        var med = db.MedicationAdministrations.FirstOrDefault(x => x.Id == medicationId);
        if (med is null) return null;
        if (med.Status != MedicationStatus.Due)
            throw new InvalidOperationException("Medication administration is locked after it is no longer due.");

        med.AdministeredAtUtc = DateTime.UtcNow;
        med.Status = MedicationStatus.Administered;
        med.Remarks = request.Remarks;
        med.AdministeredBy = actorUserId;
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Nursing.Medication.Administer", "MedicationAdministration", med.Id.ToString(), med.MedicationName);
        return Map(med);
    }

    public IReadOnlyCollection<MedicationAdministrationDto> GetMedicationDueList(DateTime asOfUtc)
    {
        var overdue = db.MedicationAdministrations.AsNoTracking()
            .Where(x => x.Status == MedicationStatus.Due && x.DueAtUtc < asOfUtc.AddMinutes(-30))
            .ToArray();

        foreach (var med in overdue)
        {
            var hasActiveAlert = db.CareAlerts.Any(x =>
                x.AdmissionId == med.AdmissionId &&
                !x.Resolved &&
                x.Message == $"Overdue medication: {med.MedicationName}");
            if (!hasActiveAlert)
                AddAlert(med.AdmissionId, AlertSeverity.Critical, $"Overdue medication: {med.MedicationName}");
        }

        if (db.ChangeTracker.HasChanges()) db.SaveChanges();

        return db.MedicationAdministrations.AsNoTracking()
            .Where(x => x.Status == MedicationStatus.Due)
            .OrderBy(x => x.DueAtUtc)
            .Select(x => Map(x))
            .ToArray();
    }

    public VitalsScheduleDto ScheduleVitals(ScheduleVitalsRequest request, Guid? actorUserId)
    {
        var vitals = new VitalsScheduleEntity
        {
            AdmissionId = request.AdmissionId,
            DueAtUtc = request.DueAtUtc,
            CreatedBy = actorUserId
        };

        db.VitalsSchedules.Add(vitals);
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Nursing.Vitals.Schedule", "VitalsSchedule", vitals.Id.ToString(), vitals.DueAtUtc.ToString("O"));
        return Map(vitals);
    }

    public VitalsScheduleDto? RecordVitals(Guid vitalsScheduleId, RecordVitalsRequest request, Guid? actorUserId)
    {
        var vitals = db.VitalsSchedules.FirstOrDefault(x => x.Id == vitalsScheduleId);
        if (vitals is null) return null;
        if (vitals.Recorded)
            throw new InvalidOperationException("Vitals schedule is locked after vitals are recorded.");

        vitals.TemperatureC = request.TemperatureC;
        vitals.Pulse = request.Pulse;
        vitals.SystolicBp = request.SystolicBp;
        vitals.DiastolicBp = request.DiastolicBp;
        vitals.SpO2 = request.SpO2;
        vitals.Recorded = true;
        vitals.RecordedBy = actorUserId;

        if (request.SpO2 is not null && request.SpO2 < 90)
            AddAlert(vitals.AdmissionId, AlertSeverity.Critical, "Critical SpO2 recorded");

        db.SaveChanges();
        auditLogService.Add(actorUserId, "Nursing.Vitals.Record", "VitalsSchedule", vitals.Id.ToString(), "Vitals recorded");
        return Map(vitals);
    }

    public IReadOnlyCollection<VitalsScheduleDto> GetVitalsSchedules(Guid admissionId)
        => db.VitalsSchedules.AsNoTracking()
            .Where(x => x.AdmissionId == admissionId)
            .OrderByDescending(x => x.DueAtUtc)
            .Select(x => Map(x))
            .ToArray();

    public IntakeOutputDto AddIntakeOutput(AddIntakeOutputRequest request, Guid? actorUserId)
    {
        var io = new IntakeOutputEntity
        {
            AdmissionId = request.AdmissionId,
            IntakeMl = request.IntakeMl,
            OutputMl = request.OutputMl,
            Notes = request.Notes,
            CreatedBy = actorUserId
        };

        db.IntakeOutputs.Add(io);
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Nursing.IO", "IntakeOutput", io.Id.ToString(), $"I:{io.IntakeMl} O:{io.OutputMl}");
        return Map(io);
    }

    public IReadOnlyCollection<IntakeOutputDto> GetIntakeOutput(Guid admissionId)
        => db.IntakeOutputs.AsNoTracking()
            .Where(x => x.AdmissionId == admissionId)
            .OrderByDescending(x => x.AtUtc)
            .Select(x => Map(x))
            .ToArray();

    public ShiftHandoverDto AddShiftHandover(AddShiftHandoverRequest request, Guid? actorUserId)
    {
        var handover = new ShiftHandoverEntity
        {
            AdmissionId = request.AdmissionId,
            FromShift = request.FromShift,
            ToShift = request.ToShift,
            Summary = request.Summary,
            CreatedBy = actorUserId
        };

        db.ShiftHandovers.Add(handover);
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Nursing.Handover", "ShiftHandover", handover.Id.ToString(), handover.Summary);
        return Map(handover);
    }

    public IReadOnlyCollection<ShiftHandoverDto> GetShiftHandovers(Guid admissionId)
        => db.ShiftHandovers.AsNoTracking()
            .Where(x => x.AdmissionId == admissionId)
            .OrderByDescending(x => x.AtUtc)
            .Select(x => Map(x))
            .ToArray();

    public IReadOnlyCollection<CareAlertDto> GetActiveAlerts()
        => db.CareAlerts.AsNoTracking()
            .Where(x => !x.Resolved)
            .OrderByDescending(x => x.AtUtc)
            .Select(x => Map(x))
            .ToArray();

    public IReadOnlyCollection<object> GetAssignedPatients()
    {
        var admitted = admissions.GetAdmissions().Where(x => x.Status == "Admitted").ToArray();
        var admissionIds = admitted.Select(x => x.Id).ToArray();
        var pendingMedications = db.MedicationAdministrations.AsNoTracking()
            .Where(x => admissionIds.Contains(x.AdmissionId) && x.Status == MedicationStatus.Due)
            .GroupBy(x => x.AdmissionId)
            .Select(x => new { AdmissionId = x.Key, Count = x.Count() })
            .ToDictionary(x => x.AdmissionId, x => x.Count);
        var pendingVitals = db.VitalsSchedules.AsNoTracking()
            .Where(x => admissionIds.Contains(x.AdmissionId) && !x.Recorded)
            .GroupBy(x => x.AdmissionId)
            .Select(x => new { AdmissionId = x.Key, Count = x.Count() })
            .ToDictionary(x => x.AdmissionId, x => x.Count);

        return admitted.Select(a => (object)new
        {
            a.Id,
            a.PatientId,
            a.PatientName,
            a.CurrentBedId,
            pendingMedications = pendingMedications.GetValueOrDefault(a.Id),
            pendingVitals = pendingVitals.GetValueOrDefault(a.Id)
        }).ToArray();
    }

    public IReadOnlyCollection<object> GetTransferDischargePreparation()
    {
        var board = admissions.GetBedBoard(null);
        return board.Where(b => b.Status == BedStatus.TransferPending || b.Status == BedStatus.DischargePending)
            .Select(b => (object)new { b.BedId, b.PatientName, b.Status, b.Room, b.Floor })
            .ToArray();
    }

    private void AddAlert(Guid admissionId, AlertSeverity severity, string message)
        => db.CareAlerts.Add(new CareAlertEntity
        {
            AdmissionId = admissionId,
            Severity = severity,
            Message = message
        });

    private static NursingNoteDto Map(NursingNoteEntity x)
        => new(x.Id, x.AdmissionId, x.AtUtc, x.NurseName, x.Note, x.LinkedDoctorRoundEncounterId);

    private static MedicationAdministrationDto Map(MedicationAdministrationEntity x)
        => new(x.Id, x.AdmissionId, x.MedicationName, x.Dose, x.DueAtUtc, x.AdministeredAtUtc, x.Status, x.Remarks);

    private static VitalsScheduleDto Map(VitalsScheduleEntity x)
        => new(x.Id, x.AdmissionId, x.DueAtUtc, x.TemperatureC, x.Pulse, x.SystolicBp, x.DiastolicBp, x.SpO2, x.Recorded);

    private static IntakeOutputDto Map(IntakeOutputEntity x)
        => new(x.Id, x.AdmissionId, x.AtUtc, x.IntakeMl, x.OutputMl, x.Notes);

    private static ShiftHandoverDto Map(ShiftHandoverEntity x)
        => new(x.Id, x.AdmissionId, x.AtUtc, x.FromShift, x.ToShift, x.Summary);

    private static CareAlertDto Map(CareAlertEntity x)
        => new(x.Id, x.AdmissionId, x.Severity, x.Message, x.AtUtc, x.Resolved);
}
