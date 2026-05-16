namespace Mediflow.Application.Abstractions.Patient;

public enum AppointmentState { Scheduled, CheckedIn, Completed, Cancelled }
public enum QueueState { Waiting, InProgress, Served, Skipped }

public sealed record PatientDto(Guid Id, string PatientCode, string FullName, DateOnly DateOfBirth, string Phone, string Gender, string Address, DateTime CreatedAtUtc);
public sealed record AppointmentDto(Guid Id, Guid PatientId, string PatientName, DateOnly AppointmentDate, string Department, string DoctorName, AppointmentState State, int TokenNumber, QueueState QueueState, DateTime CreatedAtUtc);

public sealed record CreatePatientRequest(string FullName, DateOnly DateOfBirth, string Phone, string Gender, string Address);
public sealed record UpdatePatientRequest(string FullName, DateOnly DateOfBirth, string Phone, string Gender, string Address);
public sealed record PatientSearchRequest(string? PatientCode, string? Name, string? Phone, DateOnly? DateOfBirth);

public sealed record CreateAppointmentRequest(Guid PatientId, DateOnly AppointmentDate, string Department, string DoctorName);
public sealed record UpdateAppointmentStateRequest(AppointmentState State);
public sealed record UpdateQueueStateRequest(QueueState State);

public interface IPatientWorkflowService
{
    PatientDto CreatePatient(CreatePatientRequest request, Guid? actorUserId);
    PatientDto? UpdatePatient(Guid patientId, UpdatePatientRequest request, Guid? actorUserId);
    bool DeletePatient(Guid patientId, Guid? actorUserId);
    PatientDto? GetPatient(Guid patientId);
    IReadOnlyCollection<PatientDto> SearchPatients(PatientSearchRequest request);

    AppointmentDto CreateAppointment(CreateAppointmentRequest request, Guid? actorUserId);
    AppointmentDto? UpdateAppointmentState(Guid appointmentId, UpdateAppointmentStateRequest request, Guid? actorUserId);
    IReadOnlyCollection<AppointmentDto> GetDailyAppointments(DateOnly date);

    AppointmentDto? UpdateQueueState(Guid appointmentId, UpdateQueueStateRequest request, Guid? actorUserId);
    IReadOnlyCollection<AppointmentDto> GetQueue(DateOnly date);
}
