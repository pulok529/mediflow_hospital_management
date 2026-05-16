using FluentValidation;
using Mediflow.Application.Abstractions.Patient;

namespace Mediflow.Application.Patient.Validators;

public sealed class CreatePatientRequestValidator : AbstractValidator<CreatePatientRequest>
{
    public CreatePatientRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Gender).NotEmpty().MaximumLength(20);
        RuleFor(x => x.DateOfBirth).LessThan(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));
    }
}

public sealed class CreateAppointmentRequestValidator : AbstractValidator<CreateAppointmentRequest>
{
    public CreateAppointmentRequestValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.Department).NotEmpty().MaximumLength(120);
        RuleFor(x => x.DoctorName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.AppointmentDate).GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow.Date));
    }
}
