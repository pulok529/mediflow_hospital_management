using FluentValidation;
using Mediflow.Application.Abstractions.Admission;

namespace Mediflow.Application.Admission.Validators;

public sealed class CreateAdmissionRequestValidator : AbstractValidator<CreateAdmissionRequest>
{
    public CreateAdmissionRequestValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.PatientName).NotEmpty();
        RuleFor(x => x.Source).NotEmpty();
        RuleFor(x => x.DepositAmount).GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateBedRequestValidator : AbstractValidator<CreateBedRequest>
{
    public CreateBedRequestValidator()
    {
        RuleFor(x => x.RoomId).NotEmpty();
        RuleFor(x => x.BedNumber).NotEmpty().MaximumLength(30);
    }
}
