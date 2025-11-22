using SchoolMS.Application.Features.Assignments.Dtos;
using SchoolMS.Domain.Assignments;
using SchoolMS.Domain.Classes;

namespace SchoolMS.Application.Features.Assignments.Commands.CreateAssignment;

public class CreateAssignmentCommandHandler(IAppDbContext context, IUser user) : IRequestHandler<CreateAssignmentCommand, Result<AssignmentDto>>
{
    public async Task<Result<AssignmentDto>> Handle(CreateAssignmentCommand command, CancellationToken cancellationToken)
    {
        var classExist = await context.Classes.AnyAsync(c => c.Id == command.ClassId && c.TeacherId == Guid.Parse(user.Id));

        if (!classExist) return ClassErrors.NotFound;

        var dateNow = DateOnly.FromDateTime(DateTime.UtcNow);

        var assignment = Assignment.Create(Guid.NewGuid(), command.ClassId,
            command.Title, command.Description,
            command.DueDate, dateNow, Guid.Parse(user.Id), dateNow);

        if (assignment.IsError) return assignment.Errors;

        context.Assignments.Add(assignment.Value);

        await context.SaveChangesAsync(cancellationToken);

        return new AssignmentDto
        {
            Id = assignment.Value.Id,
            Title = command.Title,
            Description = command.Description,
            DueDate = command.DueDate
        };
    }
}
