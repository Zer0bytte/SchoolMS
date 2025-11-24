using MediatR;
using SchoolMS.Api.Extensions;
using SchoolMS.Application.Features.Classes.Commands.AssignStudents;
using SchoolMS.Application.Features.Classes.Commands.CreateClass;
using SchoolMS.Application.Features.Classes.Commands.DeactivateClass;
using SchoolMS.Application.Features.Classes.Commands.UpdateClass;
using SchoolMS.Application.Features.Classes.Dtos;
using SchoolMS.Application.Features.Classes.Queries.GetTeacherClasses;
using SchoolMS.Contracts.Classes;

namespace SchoolMS.Api.Endpoints;

public static class TeacherClassEndpoints
{
    public static void MapTeacherClassEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/teacher/classes")
               .WithTags("Teacher - Classes").RequireAuthorization("Teacher");

        group.MapGet("", GetClasses);

        group.MapPost("", CreateClass)
            .WithSummary("Create a class")
            .WithDescription("Creates a new class under the teacher, assigning course, semester, and schedule details.")
            .Accepts<CreateClassRequest>("application/json")
            .Produces<ClassDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPatch("/{classId:guid}", UpdateClass)
            .WithSummary("Update class details")
            .WithDescription("Updates class name, semester, or schedule information.")
            .Accepts<UpdateClassRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("/{classId:guid}/assign-students", AssignStudents)
            .WithSummary("Assign students to a class")
            .WithDescription("Assigns one or more students to a specific class.")
            .Accepts<AssignStudentsRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapDelete("/{classId:guid}/decativate", DeactivateClass)
            .WithSummary("Deactivate a class")
            .WithDescription("Marks the class as inactive so it can no longer be used.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetClasses([AsParameters] GetClassesRequest request, ISender sender)
    {
        var query = new GetTeacherClassesQuery()
        {
            Cursor = request.Cursor,
            Limit = request.Limit,
        };

        var result = await sender.Send(query);

        return result.Match
         (result => Results.Ok(result),
         error => error.ToProblem());
    }

    private static async Task<IResult> DeactivateClass(Guid classId, ISender sender)
    {
        var command = new DeactivateClassCommand
        {
            Id = classId
        };

        var result = await sender.Send(command);

        return result.Match
            (result => Results.NoContent(),
            error => error.ToProblem());
    }

    private static async Task<IResult> AssignStudents(Guid classId, AssignStudentsRequest request, ISender sender)
    {
        var command = new AssignStudentsCommand
        {
            ClassId = classId,
            StudentIds = request.StudentIds,
        };

        var result = await sender.Send(command);

        return result.Match
            (result => Results.NoContent(),
            error => error.ToProblem());
    }

    private static async Task<IResult> UpdateClass(Guid classId, UpdateClassRequest request, ISender sender)
    {
        var command = new UpdateClassCommand
        {
            Id = classId,
            Name = request.Name,
            Semester = request.Semester,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
        };

        var result = await sender.Send(command);

        return result.Match
            (result => Results.NoContent(),
            error => error.ToProblem());
    }

    private static async Task<IResult> CreateClass(CreateClassRequest request, ISender sender)
    {
        var command = new CreateClassCommand
        {
            Name = request.Name,
            CourseId = request.CourseId,
            Semester = request.Semester,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
        };

        var result = await sender.Send(command);

        return result.Match
            (result => Results.Created($"/api/teacher/classes/{result.Id}", result),
            error => error.ToProblem());
    }


}
