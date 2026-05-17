using Mediflow.Application.Abstractions.Audit;
using Mediflow.Application.Abstractions.Patient;
using Mediflow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Mediflow.Infrastructure.Patient;

internal sealed class DatabasePatientWorkflowService(AppDbContext db, IAuditLogService auditLogService) : IPatientWorkflowService
{
    public PatientDto CreatePatient(CreatePatientRequest request, Guid? actorUserId)
    {
        var duplicate = db.Patients.AsNoTracking().FirstOrDefault(p =>
            p.FullName == request.FullName &&
            p.Phone == request.Phone &&
            p.DateOfBirth == request.DateOfBirth);

        if (duplicate is not null)
        {
            throw new InvalidOperationException("Potential duplicate patient found. Please search before creating.");
        }

        var today = DateTime.UtcNow;
        var countToday = db.Patients.Count(x => x.CreatedAtUtc.Date == today.Date) + 1;
        var patient = new PatientEntity
        {
            PatientCode = $"PT-{today:yyyyMMdd}-{countToday:0000}",
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth,
            Phone = request.Phone,
            Gender = request.Gender,
            Address = request.Address,
            CreatedAtUtc = today,
            CreatedBy = actorUserId
        };

        db.Patients.Add(patient);
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Patient.Create", "Patient", patient.Id.ToString(), $"Registered patient {patient.PatientCode}");
        return MapPatient(patient);
    }

    public PatientDto? UpdatePatient(Guid patientId, UpdatePatientRequest request, Guid? actorUserId)
    {
        var patient = db.Patients.FirstOrDefault(x => x.Id == patientId);
        if (patient is null) return null;

        patient.FullName = request.FullName;
        patient.DateOfBirth = request.DateOfBirth;
        patient.Phone = request.Phone;
        patient.Gender = request.Gender;
        patient.Address = request.Address;
        patient.UpdatedAtUtc = DateTime.UtcNow;
        patient.UpdatedBy = actorUserId;
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Patient.Update", "Patient", patient.Id.ToString(), $"Updated patient {patient.PatientCode}");
        return MapPatient(patient);
    }

    public bool DeletePatient(Guid patientId, Guid? actorUserId)
    {
        var patient = db.Patients.FirstOrDefault(x => x.Id == patientId);
        if (patient is null) return false;

        db.Patients.Remove(patient);
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Patient.Delete", "Patient", patientId.ToString(), "Deleted patient record");
        return true;
    }

    public PatientDto? GetPatient(Guid patientId)
    {
        var patient = db.Patients.AsNoTracking().FirstOrDefault(x => x.Id == patientId);
        return patient is null ? null : MapPatient(patient);
    }

    public IReadOnlyCollection<PatientDto> SearchPatients(PatientSearchRequest request)
    {
        var query = db.Patients.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.PatientCode))
            query = query.Where(x => x.PatientCode.Contains(request.PatientCode));
        if (!string.IsNullOrWhiteSpace(request.Name))
            query = query.Where(x => x.FullName.Contains(request.Name));
        if (!string.IsNullOrWhiteSpace(request.Phone))
            query = query.Where(x => x.Phone.Contains(request.Phone));
        if (request.DateOfBirth is not null)
            query = query.Where(x => x.DateOfBirth == request.DateOfBirth);

        return query.OrderByDescending(x => x.CreatedAtUtc).Select(x => MapPatient(x)).ToArray();
    }

    public AppointmentDto CreateAppointment(CreateAppointmentRequest request, Guid? actorUserId)
    {
        var patient = db.Patients.FirstOrDefault(x => x.Id == request.PatientId)
            ?? throw new InvalidOperationException("Patient not found.");

        var token = db.Appointments.Count(x =>
            x.AppointmentDate == request.AppointmentDate &&
            x.Department == request.Department &&
            x.DoctorName == request.DoctorName) + 1;

        var appointment = new AppointmentEntity
        {
            PatientId = patient.Id,
            PatientName = patient.FullName,
            AppointmentDate = request.AppointmentDate,
            Department = request.Department,
            DoctorName = request.DoctorName,
            State = AppointmentState.Scheduled,
            TokenNumber = token,
            QueueState = QueueState.Waiting,
            CreatedAtUtc = DateTime.UtcNow,
            CreatedBy = actorUserId
        };

        db.Appointments.Add(appointment);
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Appointment.Create", "Appointment", appointment.Id.ToString(), $"Booked token {token} for {patient.PatientCode}");
        return MapAppointment(appointment);
    }

    public AppointmentDto? UpdateAppointmentState(Guid appointmentId, UpdateAppointmentStateRequest request, Guid? actorUserId)
    {
        var appointment = db.Appointments.FirstOrDefault(x => x.Id == appointmentId);
        if (appointment is null) return null;

        appointment.State = request.State;
        appointment.UpdatedAtUtc = DateTime.UtcNow;
        appointment.UpdatedBy = actorUserId;
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Appointment.State", "Appointment", appointment.Id.ToString(), $"Appointment -> {request.State}");
        return MapAppointment(appointment);
    }

    public IReadOnlyCollection<AppointmentDto> GetDailyAppointments(DateOnly date)
        => db.Appointments
            .AsNoTracking()
            .Where(x => x.AppointmentDate == date)
            .OrderBy(x => x.TokenNumber)
            .Select(x => MapAppointment(x))
            .ToArray();

    public AppointmentDto? UpdateQueueState(Guid appointmentId, UpdateQueueStateRequest request, Guid? actorUserId)
    {
        var appointment = db.Appointments.FirstOrDefault(x => x.Id == appointmentId);
        if (appointment is null) return null;

        appointment.QueueState = request.State;
        if (request.State == QueueState.Served)
        {
            appointment.State = AppointmentState.Completed;
        }

        appointment.UpdatedAtUtc = DateTime.UtcNow;
        appointment.UpdatedBy = actorUserId;
        db.SaveChanges();
        auditLogService.Add(actorUserId, "Queue.State", "Appointment", appointment.Id.ToString(), $"Queue -> {request.State}");
        return MapAppointment(appointment);
    }

    public IReadOnlyCollection<AppointmentDto> GetQueue(DateOnly date) => GetDailyAppointments(date);

    private static PatientDto MapPatient(PatientEntity p)
        => new(p.Id, p.PatientCode, p.FullName, p.DateOfBirth, p.Phone, p.Gender, p.Address, p.CreatedAtUtc);

    private static AppointmentDto MapAppointment(AppointmentEntity a)
        => new(a.Id, a.PatientId, a.PatientName, a.AppointmentDate, a.Department, a.DoctorName, a.State, a.TokenNumber, a.QueueState, a.CreatedAtUtc);
}
