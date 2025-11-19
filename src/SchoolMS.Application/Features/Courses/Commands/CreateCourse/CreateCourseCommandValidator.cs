
using FluentValidation;

namespace SchoolMS.Application.Features.Courses.Commands.CreateCourse;

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.Name)
      .NotEmpty().WithMessage("Course name is required.")
      .MaximumLength(200).WithMessage("Course name must not exceed 200 characters.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Course code is required.")
            .MaximumLength(20).WithMessage("Course code must not exceed 20 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Department is required.");

        RuleFor(x => x.Credits)
            .GreaterThan(0).WithMessage("Credits must be greater than zero.");
    }
}