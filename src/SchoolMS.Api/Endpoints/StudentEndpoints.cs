using MediatR;
using SchoolMS.Api.Extensions;
using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Users.Dtos;
using SchoolMS.Application.Features.Users.Queries.GetStudents;

namespace SchoolMS.Api.Endpoints;

public static class StudentEndpoints
{
    public static void MapStudentEndpoints(this IEndpointRouteBuilder app, Asp.Versioning.Builder.ApiVersionSet vset)
    {
        var v1 = app
            .MapGroup("/api/v{version:apiVersion}/teacher/students")
            .WithApiVersionSet(vset)
            .RequireAuthorization("Teacher")
            .WithTags("Students");

        v1.MapGet("", GetStudents)
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
