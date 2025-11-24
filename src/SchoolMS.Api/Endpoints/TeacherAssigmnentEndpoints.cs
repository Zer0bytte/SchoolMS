using MediatR;
using SchoolMS.Api.Extensions;
using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Assignments.Commands.CreateAssignment;
using SchoolMS.Application.Features.Assignments.Commands.GradeAssignement;
using SchoolMS.Application.Features.Assignments.Dtos;
using SchoolMS.Application.Features.Assignments.Qureies.GetAssignments;
using SchoolMS.Application.Features.Assignments.Qureies.GetAssignmentSubmissions;
using SchoolMS.Contracts.Assignments;

namespace SchoolMS.Api.Endpoints;

public static class TeacherAssigmnentEndpoints
{
    public static void MapTeacherassignmentEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/teacher/assignments")
                   .RequireAuthorization("Teacher")
                   .WithTags("Teacher Assignments");


        group.MapPost("", CreateAssignment)
            .WithName("CreateAssignment")
            .WithSummary("Create a new assignment for a class")
            .WithDescription("""
                Allows the authenticated teacher to create a new assignment for a specific class.  
                Returns the created assignment details including its generated ID.
                """)
            .Accepts<CreateAssignmentRequest>("application/json")
            .Produces<AssignmentDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("/{classId:guid}", GetAssignments)
            .WithName("GetAssignments")
            .WithSummary("Retrieve assignments for a class")
            .WithDescription("""
                Returns a list of assignments for the specified class.
                Supports cursor-based pagination using query parameters:
                - `cursor` (optional)
                - `limit` (optional)
                """)
            .Produces<CursorResult<AssignmentDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError);


        group.MapPost("/{id:guid}/grade", GradeAssignment)
            .WithName("GradeAssignment")
            .WithSummary("Grade a student submission")
            .WithDescription("Allows the teacher to grade a specific student submission with a grade and optional remarks.")
            .Accepts<GradeAssignmentRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);


        group.MapGet("/{id:guid}/submissions", GetSubmissions)
            .WithName("GetAssignmentSubmissions")
            .WithSummary("Retrieve all submissions for a specific assignment")
            .WithDescription("Returns a list of submissions for the given assignment ID.")
            .Produces<List<SubmissionDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetSubmissions(Guid id, ISender sender)
    {
        GetAssignmentSubmissionsQuery query = new GetAssignmentSubmissionsQuery
        {
            AssignmentId = id,
        };

        Domain.Common.Results.Result<List<SubmissionDto>> result = await sender.Send(query);

        return result.Match
              (result => Results.Ok(result),
              error => error.ToProblem());
    }

    private static async Task<IResult> GradeAssignment(Guid id, GradeAssignmentRequest request, ISender sender)
    {
        GradeAssignmentCommand command = new GradeAssignmentCommand
        {
            SubmissionId = id,
            Grade = request.Grade,
            Remarks = request.Remarks,
        };

        Domain.Common.Results.Result<Domain.Common.Results.Success> result = await sender.Send(command);

        return result.Match
              (result => Results.NoContent(),
              error => error.ToProblem());
    }

    private static async Task<IResult> GetAssignments(Guid classId, [AsParameters] GetAssignmentsRequest request, ISender sender)
    {
        GetAssignmentsQuery query = new GetAssignmentsQuery
        {
            ClassId = classId,
            Cursor = request.Cursor,
            Limit = request.Limit
        };

        Domain.Common.Results.Result<CursorResult<AssignmentDto>> result = await sender.Send(query);

        return result.Match
              (result => Results.Ok(result),
              error => error.ToProblem());
    }

    private static async Task<IResult> CreateAssignment(CreateAssignmentRequest request, ISender sender)
    {
        CreateAssignmentCommand command = new CreateAssignmentCommand
        {
            ClassId = request.ClassId,
            Description = request.Description,
            DueDate = request.DueDate,
            Title = request.Title,
        };

        Domain.Common.Results.Result<AssignmentDto> result = await sender.Send(command);

        return result.Match
              (result => Results.Created($"/api/teacher/assignments/{result.Id}", result),
              error => error.ToProblem());

    }
}
