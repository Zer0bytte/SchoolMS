using SchoolMS.Application.Common.Errors;
using SchoolMS.Application.Features.Classes.Dtos;
using SchoolMS.Domain.Classes;

namespace SchoolMS.Application.Features.Classes.Commands.UpdateClass;

public class UpdateClassCommandHandler(IAppDbContext context, IUser user)
    : IRequestHandler<UpdateClassCommand, Result<ClassDto>>
{
    public async Task<Result<ClassDto>> Handle(UpdateClassCommand request, CancellationToken cancellationToken)
    {

        if (string.IsNullOrWhiteSpace(user.Id))
            return ApplicationErrors.UserNotFound;


        var existingClass = await context.Classes
            .Include(c => c.Course)
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.IsActive, cancellationToken);

        if (existingClass is null)
            return ClassErrors.NotFound;


        var updateResult = existingClass.Update(
            request.Name,
            request.Semester,
            request.StartDate,
            request.EndDate);

        if (updateResult.IsError)
            return updateResult.Errors;

        await context.SaveChangesAsync(cancellationToken);

        var dto = new ClassDto
        {
            Id = existingClass.Id,
            Name = existingClass.Name,
            CourseId = existingClass.CourseId,
            CourseName = existingClass.Course.Name,
            Semester = existingClass.Semester,
            StartDate = existingClass.StartDate,
            EndDate = existingClass.EndDate
        };

        return dto;
    }
}