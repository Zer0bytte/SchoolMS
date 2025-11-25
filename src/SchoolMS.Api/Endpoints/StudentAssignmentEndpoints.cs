
using MediatR;
using SchoolMS.Api.Extensions;
using SchoolMS.Application.Common.Interfaces;
using SchoolMS.Application.Features.Assignments.Commands.SubmitAssignment;
using SchoolMS.Application.Features.Assignments.Dtos;
using SchoolMS.Application.Features.Assignments.Qureies.GetGrades;
using SchoolMS.Application.Features.Assignments.Qureies.GetStudentAssignments;

namespace SchoolMS.Api.Endpoints;

public static class StudentAssignmentEndpoints
{
    public static void MapStudentAssignmentEndpoints(this IEndpointRouteBuilder app, Asp.Versioning.Builder.ApiVersionSet vset)
    {
        RouteGroupBuilder v1 = app.MapGroup("/api/v{version:apiVersion}/student")
                   .WithApiVersionSet(vset)
                   .RequireAuthorization("Student")
                   .WithTags("Student Assignments");

        v1.MapPost("/assignments/{id:guid}/submit", SubmitAssignment)
               .DisableAntiforgery()
               .WithName("SubmitAssignment")
               .WithSummary("Submit an assignment")
               .WithDescription("""
                Allows the authenticated student to upload and submit an assignment file.
                A file is required. The assignment ID is passed in the route.
                """)
               .Accepts<IFormFile>("multipart/form-data")
               .Produces(StatusCodes.Status204NoContent)
               .Produces(StatusCodes.Status400BadRequest)
               .Produces(StatusCodes.Status401Unauthorized)
               .Produces(StatusCodes.Status403Forbidden)
               .Produces(StatusCodes.Status404NotFound)
               .Produces(StatusCodes.Status500InternalServerError);

        v1.MapGet("/assignments", GetAssignments)
            .WithName("GetStudentAssignments")
            .WithSummary("Get assignments for the authenticated student")
            .WithDescription("""
                Retrieves all assignments assigned to the authenticated student.  
                Optionally, results can be filtered by class ID.
                """)
            .Produces<List<StudentAssignmentDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError);

        v1.MapGet("/grades", GetGrades)
            .WithName("GetStudentGrades")
            .WithSummary("Get assignment grades for the authenticated student")
            .WithDescription("""
                Retrieves all graded assignments for the authenticated student.
                """)
            .Produces<List<GradeDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> GetGrades(ISender sender)
    {
        GetGradesQuery query = new GetGradesQuery();
        Domain.Common.Results.Result<List<GradeDto>> result = await sender.Send(query);

        return result.Match
             (result => Results.Ok(result),
             error => error.ToProblem());
    }

    private static async Task<IResult> GetAssignments(Guid? classId, ISender sender)
    {
        GetStudentAssignmentsQuery query = new GetStudentAssignmentsQuery
        {
            ClassId = classId
        };

        Domain.Common.Results.Result<List<StudentAssignmentDto>> result = await sender.Send(query);

        return result.Match
             (result => Results.Ok(result),
             error => error.ToProblem());
    }

    private static async Task<IResult> SubmitAssignment(Guid id, IFormFile file, ISender sender)
    {
        if (file is null || file.Length == 0)
            return Results.BadRequest("File is required.");

        SubmitAssignmentCommand command = new SubmitAssignmentCommand
        {
            AssignmentId = id,
            File = new FileData
            {
                FileName = file.FileName,
                Content = file.OpenReadStream()
            }
        };

        Domain.Common.Results.Result<Domain.Common.Results.Success> result = await sender.Send(command);

        return result.Match
              (result => Results.NoContent(),
              error => error.ToProblem());
    }
}
