using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Domain.Classes;
using SchoolMS.Domain.Notifications;
using SchoolMS.Domain.Notifications.Enums;
using SchoolMS.Domain.Users.Enums;

namespace SchoolMS.Application.Features.Notifications.Commands.SendNotification;

public class SendNotificationCommandHandler(
    IAppDbContext context,
    ILogger<SendNotificationCommandHandler> logger
) : IRequestHandler<SendNotificationCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(SendNotificationCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Send notification started. IsClass={IsClass}, ClassId={ClassId}, StudentId={StudentId}, Title={Title}",
            command.IsClass, command.ClassId, command.StudentId, command.Title
        );

        List<Guid> studentIds = [];
        RecipientRole recipientRole = RecipientRole.Student;

        if (command.IsClass)
        {
            recipientRole = RecipientRole.Class;

            bool cls = await context.Classes
                .AnyAsync(c => c.Id == command.ClassId, cancellationToken);

            if (!cls)
            {
                logger.LogWarning(
                    "Send notification failed: class not found. ClassId={ClassId}, Title={Title}",
                    command.ClassId, command.Title
                );
                return ClassErrors.NotFound;
            }

            studentIds.AddRange(
                context.StudentClasses
                    .Where(s => s.ClassId == command.ClassId)
                    .Select(s => s.StudentId)
            );

            logger.LogDebug(
                "Class notification target resolved. ClassId={ClassId}, StudentsCount={StudentsCount}",
                command.ClassId, studentIds.Count
            );
        }
        else
        {
            bool student = await context.Users
                .AnyAsync(
                    s => s.Id == command.StudentId && s.Role == Role.Student,
                    cancellationToken);

            if (!student)
            {
                logger.LogWarning(
                    "Send notification failed: student not found or not a student. StudentId={StudentId}, Title={Title}",
                    command.StudentId, command.Title
                );
                return ApplicationErrors.UserNotFound;
            }

            studentIds.Add(command.StudentId!.Value);

            logger.LogDebug(
                "Single-student notification target resolved. StudentId={StudentId}",
                command.StudentId
            );
        }

        int createdCount = 0;

        foreach (Guid studentId in studentIds)
        {
            Result<Notification> notification = Notification.Create(
                Guid.CreateVersion7(),
                command.Title,
                command.Message,
                recipientRole,
                studentId
            );

            if (notification.IsError)
            {
                logger.LogWarning(
                    "Send notification failed: domain validation errors. RecipientRole={RecipientRole}, StudentId={StudentId}, Errors={Errors}",
                    recipientRole, studentId, notification.Errors
                );
                return notification.Errors;
            }

            context.Notifications.Add(notification.Value);
            createdCount++;
        }

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Send notification succeeded. RecipientRole={RecipientRole}, CreatedCount={CreatedCount}, IsClass={IsClass}, ClassId={ClassId}",
            recipientRole, createdCount, command.IsClass, command.ClassId
        );

        return Result.Success;
    }
}
