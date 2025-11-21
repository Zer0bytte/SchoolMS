using FluentValidation;

namespace SchoolMS.Application.Features.Courses.Commands.UpdateCourse;

public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Course ID is required.");

        RuleFor(x => x.Name)
     .MaximumLength(200).WithMessage("Course name must not exceed 200 characters.");

        RuleFor(x => x.Code)
            .MaximumLength(20).WithMessage("Course code must not exceed 20 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.");


        RuleFor(x => x.Credits)
            .GreaterThan(0).WithMessage("Credits must be greater than zero.");
    }
}
