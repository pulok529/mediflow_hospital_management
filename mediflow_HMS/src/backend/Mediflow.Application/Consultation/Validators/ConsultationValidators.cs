using FluentValidation;
using Mediflow.Application.Abstractions.Consultation;

namespace Mediflow.Application.Consultation.Validators;

public sealed class StartEncounterRequestValidator : AbstractValidator<StartEncounterRequest>
{
    public StartEncounterRequestValidator()
    {
        RuleFor(x => x.AppointmentId).NotEmpty();
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.PatientName).NotEmpty().MaximumLength(120);
    }
}

public sealed class SaveConsultationRequestValidator : AbstractValidator<SaveConsultationRequest>
{
    public SaveConsultationRequestValidator()
    {
        RuleFor(x => x.ChiefComplaint).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Diagnosis).NotEmpty().MaximumLength(500);
        RuleForEach(x => x.Prescriptions).ChildRules(p =>
        {
            p.RuleFor(x => x.MedicineName).NotEmpty();
            p.RuleFor(x => x.Dose).NotEmpty();
            p.RuleFor(x => x.Frequency).NotEmpty();
        });
    }
}
