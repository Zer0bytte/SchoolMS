using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Features.Classes.Dtos;
using SchoolMS.Domain.Classes;
using SchoolMS.Domain.Courses;

namespace SchoolMS.Application.Features.Classes.Commands.CreateClass;

public class CreateClassCommandHandler(IAppDbContext context, IUser user) : IRequestHandler<CreateClassCommand, Result<ClassDto>>
{
    public async Task<Result<ClassDto>> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {
        var course = await context.Courses.FirstOrDefaultAsync(c => c.Id == request.CourseId);
        if (course is null)
        {
            return CourseErrors.NotFound;
        }

        if (string.IsNullOrWhiteSpace(user.Id))
        {
            return ApplicationErrors.UserNotFound;
        }

        var newClass = Class.Create(Guid.CreateVersion7(),
            request.Name,
            request.CourseId,
            Guid.Parse(user.Id!),
            request.Semester,
            request.StartDate, request.EndDate);

        if (newClass.IsError)
            return newClass.Errors;

        context.Classes.Add(newClass.Value);
        await context.SaveChangesAsync(cancellationToken);

        var classDto = new ClassDto
        {
            Id = newClass.Value.Id,
            Name = newClass.Value.Name,
            CourseId = newClass.Value.CourseId,
            CourseName = course.Name,
            Semester = newClass.Value.Semester,
            StartDate = newClass.Value.StartDate,
            EndDate = newClass.Value.EndDate,
        };

        return classDto;

    }
}
