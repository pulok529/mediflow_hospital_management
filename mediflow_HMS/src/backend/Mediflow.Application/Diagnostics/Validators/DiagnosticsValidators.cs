using FluentValidation;
using Mediflow.Application.Abstractions.Diagnostics;

namespace Mediflow.Application.Diagnostics.Validators;

public sealed class CreateLabOrderRequestValidator : AbstractValidator<CreateLabOrderRequest>
{
    public CreateLabOrderRequestValidator()
    {
        RuleFor(x => x.AdmissionId).NotEmpty();
        RuleFor(x => x.PatientName).NotEmpty();
        RuleFor(x => x.TestCode).NotEmpty();
    }
}

public sealed class EnterImagingReportRequestValidator : AbstractValidator<EnterImagingReportRequest>
{
    public EnterImagingReportRequestValidator()
    {
        RuleFor(x => x.ReportText).NotEmpty();
        RuleFor(x => x.FileReference).NotEmpty();
    }
}
