namespace SchoolMS.Application.Features.Courses.Commands.DeleteCourse;

public class DeleteCourseCommand : IRequest<Result<Success>>
{
    public Guid Id { get; set; }
}
