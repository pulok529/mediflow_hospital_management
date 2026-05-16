using Mediflow.Application.Abstractions.Patient;

namespace Mediflow.Api.Contracts.Workflow;

public sealed record CreatePatientRequest(string FullName, DateOnly DateOfBirth, string Phone, string Gender, string Address);
public sealed record UpdatePatientRequest(string FullName, DateOnly DateOfBirth, string Phone, string Gender, string Address);
public sealed record SearchPatientsRequest(string? PatientCode, string? Name, string? Phone, DateOnly? DateOfBirth);
public sealed record CreateAppointmentRequest(Guid PatientId, DateOnly AppointmentDate, string Department, string DoctorName);
public sealed record UpdateAppointmentStateRequest(AppointmentState State);
public sealed record UpdateQueueStateRequest(QueueState State);
