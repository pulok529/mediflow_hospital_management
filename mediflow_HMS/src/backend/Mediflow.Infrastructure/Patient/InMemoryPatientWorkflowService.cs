using Mediflow.Application.Abstractions.Audit;
using Mediflow.Application.Abstractions.Patient;

namespace Mediflow.Infrastructure.Patient;

internal sealed class InMemoryPatientWorkflowService(IAuditLogService auditLogService) : IPatientWorkflowService
{
    private static readonly object Sync = new();
    private static readonly List<PatientRecord> Patients = [];
    private static readonly List<AppointmentRecord> Appointments = [];

    public PatientDto CreatePatient(CreatePatientRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var duplicate = Patients.FirstOrDefault(p =>
                p.FullName.Equals(request.FullName, StringComparison.OrdinalIgnoreCase) &&
                p.Phone.Equals(request.Phone, StringComparison.OrdinalIgnoreCase) &&
                p.DateOfBirth == request.DateOfBirth);
            if (duplicate is not null)
                throw new InvalidOperationException("Potential duplicate patient found. Please search before creating.");

            var patient = new PatientRecord
            {
                Id = Guid.NewGuid(),
                PatientCode = $"PT-{DateTime.UtcNow:yyyyMMdd}-{Patients.Count + 1:0000}",
                FullName = request.FullName,
                DateOfBirth = request.DateOfBirth,
                Phone = request.Phone,
                Gender = request.Gender,
                Address = request.Address,
                CreatedAtUtc = DateTime.UtcNow
            };

            Patients.Add(patient);
            auditLogService.Add(actorUserId, "Patient.Create", "Patient", patient.Id.ToString(), $"Registered patient {patient.PatientCode}");
            return MapPatient(patient);
        }
    }

    public PatientDto? UpdatePatient(Guid patientId, UpdatePatientRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var patient = Patients.FirstOrDefault(x => x.Id == patientId);
            if (patient is null) return null;
            patient.FullName = request.FullName;
            patient.DateOfBirth = request.DateOfBirth;
            patient.Phone = request.Phone;
            patient.Gender = request.Gender;
            patient.Address = request.Address;
            auditLogService.Add(actorUserId, "Patient.Update", "Patient", patient.Id.ToString(), $"Updated patient {patient.PatientCode}");
            return MapPatient(patient);
        }
    }

    public bool DeletePatient(Guid patientId, Guid? actorUserId)
    {
        lock (Sync)
        {
            var patient = Patients.FirstOrDefault(x => x.Id == patientId);
            if (patient is null) return false;
            Patients.Remove(patient);
            auditLogService.Add(actorUserId, "Patient.Delete", "Patient", patientId.ToString(), "Deleted patient record");
            return true;
        }
    }

    public PatientDto? GetPatient(Guid patientId)
    {
        lock (Sync)
        {
            var patient = Patients.FirstOrDefault(x => x.Id == patientId);
            return patient is null ? null : MapPatient(patient);
        }
    }

    public IReadOnlyCollection<PatientDto> SearchPatients(PatientSearchRequest request)
    {
        lock (Sync)
        {
            IEnumerable<PatientRecord> query = Patients;
            if (!string.IsNullOrWhiteSpace(request.PatientCode))
                query = query.Where(x => x.PatientCode.Contains(request.PatientCode, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(request.Name))
                query = query.Where(x => x.FullName.Contains(request.Name, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(request.Phone))
                query = query.Where(x => x.Phone.Contains(request.Phone, StringComparison.OrdinalIgnoreCase));
            if (request.DateOfBirth is not null)
                query = query.Where(x => x.DateOfBirth == request.DateOfBirth);
            return query.OrderByDescending(x => x.CreatedAtUtc).Select(MapPatient).ToArray();
        }
    }

    public AppointmentDto CreateAppointment(CreateAppointmentRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var patient = Patients.FirstOrDefault(x => x.Id == request.PatientId)
                ?? throw new InvalidOperationException("Patient not found.");

            var token = Appointments.Count(x => x.AppointmentDate == request.AppointmentDate) + 1;

            var appointment = new AppointmentRecord
            {
                Id = Guid.NewGuid(),
                PatientId = patient.Id,
                PatientName = patient.FullName,
                AppointmentDate = request.AppointmentDate,
                Department = request.Department,
                DoctorName = request.DoctorName,
                State = AppointmentState.Scheduled,
                TokenNumber = token,
                QueueState = QueueState.Waiting,
                CreatedAtUtc = DateTime.UtcNow
            };

            Appointments.Add(appointment);
            auditLogService.Add(actorUserId, "Appointment.Create", "Appointment", appointment.Id.ToString(), $"Booked token {token} for {patient.PatientCode}");
            return MapAppointment(appointment);
        }
    }

    public AppointmentDto? UpdateAppointmentState(Guid appointmentId, UpdateAppointmentStateRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var appointment = Appointments.FirstOrDefault(x => x.Id == appointmentId);
            if (appointment is null) return null;
            appointment.State = request.State;
            auditLogService.Add(actorUserId, "Appointment.State", "Appointment", appointment.Id.ToString(), $"Appointment -> {request.State}");
            return MapAppointment(appointment);
        }
    }

    public IReadOnlyCollection<AppointmentDto> GetDailyAppointments(DateOnly date)
    {
        lock (Sync)
        {
            return Appointments.Where(x => x.AppointmentDate == date).OrderBy(x => x.TokenNumber).Select(MapAppointment).ToArray();
        }
    }

    public AppointmentDto? UpdateQueueState(Guid appointmentId, UpdateQueueStateRequest request, Guid? actorUserId)
    {
        lock (Sync)
        {
            var appointment = Appointments.FirstOrDefault(x => x.Id == appointmentId);
            if (appointment is null) return null;
            appointment.QueueState = request.State;
            if (request.State == QueueState.Served) appointment.State = AppointmentState.Completed;
            auditLogService.Add(actorUserId, "Queue.State", "Appointment", appointment.Id.ToString(), $"Queue -> {request.State}");
            return MapAppointment(appointment);
        }
    }

    public IReadOnlyCollection<AppointmentDto> GetQueue(DateOnly date) => GetDailyAppointments(date);

    private static PatientDto MapPatient(PatientRecord p) => new(p.Id, p.PatientCode, p.FullName, p.DateOfBirth, p.Phone, p.Gender, p.Address, p.CreatedAtUtc);
    private static AppointmentDto MapAppointment(AppointmentRecord a) => new(a.Id, a.PatientId, a.PatientName, a.AppointmentDate, a.Department, a.DoctorName, a.State, a.TokenNumber, a.QueueState, a.CreatedAtUtc);

    private sealed class PatientRecord
    {
        public Guid Id { get; set; }
        public string PatientCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
    }

    private sealed class AppointmentRecord
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public DateOnly AppointmentDate { get; set; }
        public string Department { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public AppointmentState State { get; set; }
        public int TokenNumber { get; set; }
        public QueueState QueueState { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
