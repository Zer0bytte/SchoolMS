using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolMS.Application.Common.Errors;
using SchoolMS.Domain.Classes;

namespace SchoolMS.Application.Features.Classes.Commands.DeactivateClass;

public class DeactivateClassCommandHandler(
    IAppDbContext context,
    IUser user,
    ILogger<DeactivateClassCommandHandler> logger
) : IRequestHandler<DeactivateClassCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(DeactivateClassCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(user.Id))
        {
            logger.LogWarning(
                "Deactivate class failed: user id missing. ClassId={ClassId}",
                command.Id
            );
            return ApplicationErrors.UserNotFound;
        }

        var teacherId = Guid.Parse(user.Id);

        logger.LogInformation(
            "Deactivate class started. ClassId={ClassId}, TeacherId={TeacherId}",
            command.Id, teacherId
        );

        var cls = await context.Classes
            .FirstOrDefaultAsync(c => c.Id == command.Id && c.IsActive, cancellationToken);

        if (cls is null)
        {
            logger.LogWarning(
                "Deactivate class failed: class not found or already inactive. ClassId={ClassId}, TeacherId={TeacherId}",
                command.Id, teacherId
            );
            return ClassErrors.NotFound;
        }

        if (cls.TeacherId != teacherId)
        {
            logger.LogWarning(
                "Deactivate class failed: teacher does not own class. ClassId={ClassId}, TeacherId={TeacherId}, OwnerTeacherId={OwnerTeacherId}",
                command.Id, teacherId, cls.TeacherId
            );
            return ClassErrors.NotFound;
        }

        cls.Deactivate();
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Deactivate class succeeded. ClassId={ClassId}, TeacherId={TeacherId}",
            command.Id, teacherId
        );

        return Result.Success;
    }
}
