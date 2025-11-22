using FluentValidation;

namespace SchoolMS.Application.Features.Classes.Commands.UpdateClass;

public class UpdateClassCommandValidator : AbstractValidator<UpdateClassCommand>
{
    public UpdateClassCommandValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Class name cannot exceed 100 characters.");


        RuleFor(x => x.Semester)
            .MaximumLength(50).WithMessage("Semester cannot exceed 50 characters.");


        RuleFor(x => x)
            .Must(x =>
            {
                if (x.StartDate.HasValue && x.EndDate.HasValue)
                    return x.EndDate.Value >= x.StartDate.Value;

                return true;
            })
            .WithMessage("End date must be on or after the start date.");
    }
}