
using SchoolMS.Application.Common.Errors;
using SchoolMS.Domain.Classes;
using SchoolMS.Domain.Notifications;
using SchoolMS.Domain.Notifications.Enums;
using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Application.Features.Notifications.Commands;

public class SendNotificationCommandHandler(IAppDbContext context) : IRequestHandler<SendNotificationCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(SendNotificationCommand command, CancellationToken cancellationToken)
    {
        List<Guid> studentIds = [];
        RecipientRole recipientRole = RecipientRole.Student;
        if (command.IsClass)
        {
            recipientRole = RecipientRole.Class;
            bool cls = await context.Classes.AnyAsync(c => c.Id == command.ClassId);
            if (!cls) return ClassErrors.NotFound;

            studentIds.AddRange(context.StudentClasses.Where(s => s.ClassId == command.ClassId).Select(s => s.StudentId));
        }
        else
        {
            bool student = await context.Users.AnyAsync(s => s.Id == command.StudentId && s.Role == Role.Student);
            if (!student) return ApplicationErrors.UserNotFound;
            studentIds.Add(command.StudentId!.Value);
        }

        foreach (Guid student in studentIds)
        {
            Result<Notification> notification = Notification.Create(Guid.CreateVersion7(), command.Title, command.Message, recipientRole, student);
            if (notification.IsError) return notification.Errors;

            context.Notifications.Add(notification.Value);
        }

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}
