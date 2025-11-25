
using MediatR;
using SchoolMS.Api.Extensions;
using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Courses.Commands.CreateCourse;
using SchoolMS.Application.Features.Courses.Commands.DeleteCourse;
using SchoolMS.Application.Features.Courses.Commands.UpdateCourse;
using SchoolMS.Application.Features.Courses.Dtos;
using SchoolMS.Application.Features.Courses.Queries.GetCourseById;
using SchoolMS.Application.Features.Courses.Queries.GetCourses;
using SchoolMS.Application.Features.Courses.Queries.GetTeacherCourses;
using SchoolMS.Contracts.Courses;

namespace SchoolMS.Api.Endpoints;

public static class CourseEndpoints
{
    public static void MapCourseEndpoints(this IEndpointRouteBuilder app, Asp.Versioning.Builder.ApiVersionSet vset)
    {
        RouteGroupBuilder v1 = app.MapGroup("/api/v{version:apiVersion}/admin/courses")
            .WithApiVersionSet(vset)
            .WithTags("Courses").RequireAuthorization("Admin");

        v1.MapGet("", GetCourses)
            .WithSummary("Get list of courses")
            .WithDescription("Returns a paginated list of courses optionally filtered by department or search term.")
            .Produces<CursorResult<CourseDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        v1.MapGet("/{id:guid}", GetCourseById)
            .WithSummary("Get course by ID")
            .WithDescription("Returns a specific course using its unique identifier.")
            .Produces<CourseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        v1.MapPost("", CreateCourse)
            .WithSummary("Create a new course")
            .WithDescription("Creates a new course record in the system.")
            .Accepts<CreateCourseRequest>("application/json")
            .Produces<CourseDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        v1.MapPatch("/{id:guid}", UpdateCourse)
            .WithSummary("Update an existing course")
            .WithDescription("Updates an existing course using its ID.")
            .Accepts<UpdateCourseRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        v1.MapDelete("/{id:guid}", DeleteCourse)
            .WithSummary("Delete an existing course")
            .WithDescription("Deletes an existing course using its ID.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);


        app.MapGet("/api/v{version:apiVersion}/teacher/courses", GetTeacherCourses)
            .WithApiVersionSet(vset)
            .RequireAuthorization("Teacher")
            .WithName("GetTeacherCourses")
            .WithTags("Courses")
            .WithSummary("Retrieve courses for the authenticated teacher")
            .WithDescription("Retrieves all courses that are assigned to the authenticated teacher.")
            .Produces<List<CourseDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> DeleteCourse(Guid id, ISender sender)
    {
        var result = await sender.Send(new DeleteCourseCommand { Id = id });

        return result.Match(
            _ => Results.NoContent(),
            errors => Results.Problem());
    }

    private static async Task<IResult> GetTeacherCourses(ISender sender)
    {
        var query = new GetTeacherCoursesQuery();

        Domain.Common.Results.Result<List<CourseDto>> result = await sender.Send(query);

        return result.Match(
             dto => Results.Ok(dto),
             errors => errors.ToProblem());
    }

    private static async Task<IResult> GetCourseById(Guid id, ISender sender)
    {
        var query = new GetCourseByIdQuery
        {
            Id = id
        };

        Domain.Common.Results.Result<CourseDto> result = await sender.Send(query);

        return result.Match(
             dto => Results.Ok(dto),
             errors => errors.ToProblem());

    }

    private static async Task<IResult> UpdateCourse(Guid id, UpdateCourseRequest request, ISender sender)
    {
        var command = new UpdateCourseCommand
        {
            Id = id,
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            DepartmentId = request.DepartmentId,
            Credits = request.Credits
        };

        Domain.Common.Results.Result<CourseDto> result = await sender.Send(command);

        return result.Match(
               dto => Results.NoContent(),
               errors => errors.ToProblem());
    }

    private static async Task<IResult> CreateCourse(CreateCourseRequest request, ISender sender)
    {
        var command = new CreateCourseCommand
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            DepartmentId = request.DepartmentId,
            Credits = request.Credits
        };

        Domain.Common.Results.Result<CourseDto> result = await sender.Send(command);

        return result.Match(
           dto => Results.Created($"/api/courses/{dto.Id}", dto),
           errors => errors.ToProblem());


    }

    private static async Task<IResult> GetCourses([AsParameters] GetCoursesRequest request, ISender sender)
    {
        var query = new GetCoursesQuery
        {
            DepartmentId = request.DepartmentId,
            SearchTerm = request.SearchTerm,
            Cursor = request.Cursor,
            Limit = request.Limit
        };

        Domain.Common.Results.Result<CursorResult<CourseDto>> result = await sender.Send(query);

        return result.Match(
           dto => Results.Ok(dto),
           errors => errors.ToProblem());

    }
}
