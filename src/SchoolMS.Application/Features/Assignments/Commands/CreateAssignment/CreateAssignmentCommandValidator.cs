using FluentValidation;

namespace SchoolMS.Application.Features.Assignments.Commands.CreateAssignment;

public class CreateAssignmentCommandValidator : AbstractValidator<CreateAssignmentCommand>
{
    public CreateAssignmentCommandValidator()
    {
        RuleFor(a => a.ClassId)
            .NotEmpty().WithMessage("ClassId is required.");

        RuleFor(a => a.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(a => a.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

        RuleFor(a => a.DueDate)
            .Must(BeInTheFuture)
            .WithMessage("DueDate must be in the future.");
    }

    private bool BeInTheFuture(DateOnly dueDate)
    {
        return dueDate > DateOnly.FromDateTime(DateTime.UtcNow);
    }
}