using Microsoft.Extensions.Logging;
using SchoolMS.Application.Features.Assignments.Dtos;
using SchoolMS.Domain.Assignments;
using SchoolMS.Domain.Classes;

namespace SchoolMS.Application.Features.Assignments.Commands.CreateAssignment;

public class CreateAssignmentCommandHandler(IAppDbContext context, IUser user, ILogger<CreateAssignmentCommandHandler> logger) : IRequestHandler<CreateAssignmentCommand, Result<AssignmentDto>>
{
    public async Task<Result<AssignmentDto>> Handle(CreateAssignmentCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Handling CreateAssignmentCommand for ClassId: {ClassId} by UserId: {UserId}", command.ClassId, user.Id);

        var classExist = await context.Classes.AnyAsync(c => c.Id == command.ClassId && c.TeacherId == Guid.Parse(user.Id), cancellationToken);

        if (!classExist)
        {
            logger.LogWarning("Class with Id {ClassId} not found or does not belong to teacher {UserId}", command.ClassId, user.Id);
            return ClassErrors.NotFound;
        }

        var dateNow = DateOnly.FromDateTime(DateTime.UtcNow);

        var assignment = Assignment.Create(Guid.NewGuid(), command.ClassId,
            command.Title, command.Description,
            command.DueDate, Guid.Parse(user.Id), dateNow);

        if (assignment.IsError)
        {
            logger.LogWarning("Failed to create assignment for ClassId {ClassId}. Errors: {Errors}", command.ClassId, assignment.Errors);
            return assignment.Errors;
        }

        context.Assignments.Add(assignment.Value);

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Assignment created successfully. AssignmentId: {AssignmentId}, ClassId: {ClassId}", assignment.Value.Id, command.ClassId);

        return new AssignmentDto
        {
            Id = assignment.Value.Id,
            Title = command.Title,
            Description = command.Description,
            DueDate = command.DueDate
        };
    }
}
