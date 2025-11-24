using MediatR;
using SchoolMS.Api.Extensions;
using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Assignments.Commands.CreateAssignment;
using SchoolMS.Application.Features.Assignments.Commands.GradeAssignement;
using SchoolMS.Application.Features.Assignments.Dtos;
using SchoolMS.Application.Features.Assignments.Qureies.GetAssignments;
using SchoolMS.Contracts.Assignments;

namespace SchoolMS.Api.Endpoints;

public static class TeacherAssigmnentEndpoints
{
    public static void MapTeacherassignmentEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/teacher/assignments");

        group.MapPost("", CreateAssignment);
        group.MapGet("/{classId:guid}", GetAssignments);
        group.MapPost("/{id:guid}/grade", GradeAssignment);
    }

    private static async Task<IResult> GradeAssignment(Guid id, GradeAssignmentRequest request, ISender sender)
    {
        GradeAssignmentCommand command = new GradeAssignmentCommand
        {
            SubmissionId = id,
            Grade = request.Grade,
            Remarks = request.Remarks,
            StudentId = request.StudentId
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
