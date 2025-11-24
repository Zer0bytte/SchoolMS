using FluentValidation;

namespace SchoolMS.Application.Features.Notifications.Commands;

public class SendNotificationCommandValidator : AbstractValidator<SendNotificationCommand>
{
    public SendNotificationCommandValidator()
    {

        When(c => !c.IsClass, () =>
        {
            RuleFor(x => x.StudentId)
            .NotEqual(Guid.Empty).WithMessage("Student id is required.")
            .NotEmpty().WithMessage("Student id is required.")
            .NotNull().WithMessage("Student id is required.");
        });


        When(c => c.IsClass, () =>
        {
            RuleFor(x => x.ClassId)
           .NotEqual(Guid.Empty).WithMessage("Class id is required.")
           .NotEmpty().WithMessage("Class id is required.")
           .NotNull().WithMessage("Class id is required.");
        });

        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.");
        RuleFor(x => x.Message).NotEmpty().WithMessage("Message is required.");
    }
}
