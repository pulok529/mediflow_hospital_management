using Mediflow.Application.Abstractions.Admission;
using Mediflow.Application.Abstractions.Audit;
using Mediflow.Application.Abstractions.Nursing;

namespace Mediflow.Infrastructure.Nursing;

internal sealed class InMemoryNursingWorkflowService(IAdmissionWorkflowService admissions, IAuditLogService auditLogService) : INursingWorkflowService
{
    private static readonly object Sync = new();
    private static readonly List<NursingNoteDto> Notes = [];
    private static readonly List<MedicationAdministrationDto> Medications = [];
    private static readonly List<VitalsScheduleDto> Vitals = [];
    private static readonly List<IntakeOutputDto> IOs = [];
    private static readonly List<ShiftHandoverDto> Handovers = [];
    private static readonly List<CareAlertDto> Alerts = [];

    public NursingNoteDto AddNursingNote(CreateNursingNoteRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var note = new NursingNoteDto(Guid.NewGuid(), request.AdmissionId, DateTime.UtcNow, request.NurseName, request.Note, request.LinkedDoctorRoundEncounterId);
            Notes.Add(note);
            auditLogService.Add(actorUserId, "Nursing.Note", "NursingNote", note.Id.ToString(), note.Note);
            return note;
        }
    }

    public IReadOnlyCollection<NursingNoteDto> GetNursingNotes(Guid admissionId)
        => Notes.Where(x => x.AdmissionId == admissionId).OrderByDescending(x => x.AtUtc).ToArray();

    public MedicationAdministrationDto AddMedication(AddMedicationRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var med = new MedicationAdministrationDto(Guid.NewGuid(), request.AdmissionId, request.MedicationName, request.Dose, request.DueAtUtc, null, MedicationStatus.Due, null);
            Medications.Add(med);
            if (request.DueAtUtc <= DateTime.UtcNow.AddMinutes(15))
                Alerts.Add(new CareAlertDto(Guid.NewGuid(), request.AdmissionId, AlertSeverity.Warning, $"Medication due soon: {request.MedicationName}", DateTime.UtcNow, false));
            auditLogService.Add(actorUserId, "Nursing.Medication.Add", "MedicationAdministration", med.Id.ToString(), med.MedicationName);
            return med;
        }
    }

    public MedicationAdministrationDto? AdministerMedication(Guid medicationId, AdministerMedicationRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var med = Medications.FirstOrDefault(x => x.Id == medicationId);
            if (med is null) return null;
            med = med with { AdministeredAtUtc = DateTime.UtcNow, Status = MedicationStatus.Administered, Remarks = request.Remarks };
            Replace(Medications, med, x => x.Id == medicationId);
            auditLogService.Add(actorUserId, "Nursing.Medication.Administer", "MedicationAdministration", med.Id.ToString(), med.MedicationName);
            return med;
        }
    }

    public IReadOnlyCollection<MedicationAdministrationDto> GetMedicationDueList(DateTime asOfUtc)
    {
        lock (Sync)
        {
            var overdue = Medications.Where(x => x.Status == MedicationStatus.Due && x.DueAtUtc < asOfUtc.AddMinutes(-30)).ToList();
            foreach (var m in overdue)
            {
                if (!Alerts.Any(a => a.AdmissionId == m.AdmissionId && a.Message.Contains(m.MedicationName) && !a.Resolved))
                {
                    Alerts.Add(new CareAlertDto(Guid.NewGuid(), m.AdmissionId, AlertSeverity.Critical, $"Overdue medication: {m.MedicationName}", DateTime.UtcNow, false));
                }
            }
            return Medications.Where(x => x.Status == MedicationStatus.Due).OrderBy(x => x.DueAtUtc).ToArray();
        }
    }

    public VitalsScheduleDto ScheduleVitals(ScheduleVitalsRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var vit = new VitalsScheduleDto(Guid.NewGuid(), request.AdmissionId, request.DueAtUtc, null, null, null, null, null, false);
            Vitals.Add(vit);
            auditLogService.Add(actorUserId, "Nursing.Vitals.Schedule", "VitalsSchedule", vit.Id.ToString(), vit.DueAtUtc.ToString("O"));
            return vit;
        }
    }

    public VitalsScheduleDto? RecordVitals(Guid vitalsScheduleId, RecordVitalsRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var vit = Vitals.FirstOrDefault(x => x.Id == vitalsScheduleId);
            if (vit is null) return null;
            vit = vit with
            {
                TemperatureC = request.TemperatureC,
                Pulse = request.Pulse,
                SystolicBp = request.SystolicBp,
                DiastolicBp = request.DiastolicBp,
                SpO2 = request.SpO2,
                Recorded = true
            };
            Replace(Vitals, vit, x => x.Id == vitalsScheduleId);

            if (request.SpO2 is not null && request.SpO2 < 90)
            {
                Alerts.Add(new CareAlertDto(Guid.NewGuid(), vit.AdmissionId, AlertSeverity.Critical, "Critical SpO2 recorded", DateTime.UtcNow, false));
            }
            auditLogService.Add(actorUserId, "Nursing.Vitals.Record", "VitalsSchedule", vit.Id.ToString(), "Vitals recorded");
            return vit;
        }
    }

    public IReadOnlyCollection<VitalsScheduleDto> GetVitalsSchedules(Guid admissionId)
        => Vitals.Where(x => x.AdmissionId == admissionId).OrderByDescending(x => x.DueAtUtc).ToArray();

    public IntakeOutputDto AddIntakeOutput(AddIntakeOutputRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var io = new IntakeOutputDto(Guid.NewGuid(), request.AdmissionId, DateTime.UtcNow, request.IntakeMl, request.OutputMl, request.Notes);
            IOs.Add(io);
            auditLogService.Add(actorUserId, "Nursing.IO", "IntakeOutput", io.Id.ToString(), $"I:{io.IntakeMl} O:{io.OutputMl}");
            return io;
        }
    }

    public IReadOnlyCollection<IntakeOutputDto> GetIntakeOutput(Guid admissionId)
        => IOs.Where(x => x.AdmissionId == admissionId).OrderByDescending(x => x.AtUtc).ToArray();

    public ShiftHandoverDto AddShiftHandover(AddShiftHandoverRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var handover = new ShiftHandoverDto(Guid.NewGuid(), request.AdmissionId, DateTime.UtcNow, request.FromShift, request.ToShift, request.Summary);
            Handovers.Add(handover);
            auditLogService.Add(actorUserId, "Nursing.Handover", "ShiftHandover", handover.Id.ToString(), handover.Summary);
            return handover;
        }
    }

    public IReadOnlyCollection<ShiftHandoverDto> GetShiftHandovers(Guid admissionId)
        => Handovers.Where(x => x.AdmissionId == admissionId).OrderByDescending(x => x.AtUtc).ToArray();

    public IReadOnlyCollection<CareAlertDto> GetActiveAlerts()
        => Alerts.Where(x => !x.Resolved).OrderByDescending(x => x.AtUtc).ToArray();

    public IReadOnlyCollection<object> GetAssignedPatients()
    {
        var admitted = admissions.GetAdmissions().Where(x => x.Status == "Admitted");
        return admitted.Select(a => (object)new
        {
            a.Id,
            a.PatientId,
            a.PatientName,
            a.CurrentBedId,
            pendingMedications = Medications.Count(m => m.AdmissionId == a.Id && m.Status == MedicationStatus.Due),
            pendingVitals = Vitals.Count(v => v.AdmissionId == a.Id && !v.Recorded)
        }).ToArray();
    }

    public IReadOnlyCollection<object> GetTransferDischargePreparation()
    {
        var board = admissions.GetBedBoard(null);
        return board.Where(b => b.Status == BedStatus.TransferPending || b.Status == BedStatus.DischargePending)
            .Select(b => (object)new { b.BedId, b.PatientName, b.Status, b.Room, b.Floor }).ToArray();
    }

    private static void Replace<T>(List<T> list, T updated, Func<T, bool> predicate)
    {
        var idx = list.FindIndex(x => predicate(x));
        if (idx >= 0) list[idx] = updated;
    }
}
