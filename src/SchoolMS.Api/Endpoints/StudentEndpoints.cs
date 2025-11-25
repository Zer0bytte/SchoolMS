using MediatR;
using SchoolMS.Api.Extensions;
using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Users.Dtos;
using SchoolMS.Application.Features.Users.Queries.GetStudents;

namespace SchoolMS.Api.Endpoints;

public static class StudentEndpoints
{
    public static void MapStudentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/teacher/students")
            .RequireAuthorization("Teacher")
            .WithTags("Students");

        group.MapGet("", GetStudents)
            .WithName("GetStudents")
            .WithSummary("Get students")
            .WithDescription(
                "Returns a paginated list of students using cursor-based pagination. " +
                "Pass a cursor from a previous response to get the next page."
            )
            .Produces<CursorResult<StudentDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);
    }

    private static async Task<IResult> GetStudents(int limit, string? cursor, ISender sender)
    {

        var query = new GetStudentsQuery
        {
            Cursor = cursor,
            Limit = limit
        };

        var result = await sender.Send(query);


        return result.Match(
           dto => Results.Ok(dto),
           errors => errors.ToProblem());
    }
}
