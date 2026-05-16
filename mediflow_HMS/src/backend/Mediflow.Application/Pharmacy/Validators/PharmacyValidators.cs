using FluentValidation;
using Mediflow.Application.Abstractions.Pharmacy;

namespace Mediflow.Application.Pharmacy.Validators;

public sealed class CreateMedicineRequestValidator : AbstractValidator<CreateMedicineRequest>
{
    public CreateMedicineRequestValidator()
    {
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.ReorderLevel).GreaterThanOrEqualTo(0);
    }
}

public sealed class AddBatchRequestValidator : AbstractValidator<AddBatchRequest>
{
    public AddBatchRequestValidator()
    {
        RuleFor(x => x.MedicineCode).NotEmpty();
        RuleFor(x => x.BatchNo).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
