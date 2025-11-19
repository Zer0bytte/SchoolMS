using SchoolMS.Domain.Courses;

namespace SchoolMS.Application.Features.Courses.Commands.DeleteCourse;

public class DeleteCourseCommandHandler(IAppDbContext context) : IRequestHandler<DeleteCourseCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(DeleteCourseCommand command, CancellationToken cancellationToken)
    {
        var course = await context.Courses
            .FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (course is null)
        {
            return CourseErrors.NotFound;
        }

        course.MarkAsDeleted();

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}