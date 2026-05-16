using FluentValidation;
using Mediflow.Application.Abstractions.Nursing;

namespace Mediflow.Application.Nursing.Validators;

public sealed class CreateNursingNoteRequestValidator : AbstractValidator<CreateNursingNoteRequest>
{
    public CreateNursingNoteRequestValidator()
    {
        RuleFor(x => x.AdmissionId).NotEmpty();
        RuleFor(x => x.NurseName).NotEmpty();
        RuleFor(x => x.Note).NotEmpty().MaximumLength(1000);
    }
}

public sealed class AddMedicationRequestValidator : AbstractValidator<AddMedicationRequest>
{
    public AddMedicationRequestValidator()
    {
        RuleFor(x => x.AdmissionId).NotEmpty();
        RuleFor(x => x.MedicationName).NotEmpty();
        RuleFor(x => x.Dose).NotEmpty();
    }
}
