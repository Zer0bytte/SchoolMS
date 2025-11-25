using Microsoft.Extensions.Logging;
using SchoolMS.Domain.Courses;

namespace SchoolMS.Application.Features.Courses.Commands.DeleteCourse;

public class DeleteCourseCommandHandler(
    IAppDbContext context,
    ILogger<DeleteCourseCommandHandler> logger
) : IRequestHandler<DeleteCourseCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(DeleteCourseCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Delete course started. CourseId={CourseId}",
            command.Id
        );

        var course = await context.Courses
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (course is null)
        {
            logger.LogWarning(
                "Delete course failed: course not found. CourseId={CourseId}",
                command.Id
            );
            return CourseErrors.NotFound;
        }

        course.MarkAsDeleted();

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Delete course succeeded. CourseId={CourseId}",
            command.Id
        );

        return Result.Success;
    }
}
