
using MediatR;
using SchoolMS.Api.Extensions;
using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Courses.Commands.CreateCourse;
using SchoolMS.Application.Features.Courses.Commands.UpdateCourse;
using SchoolMS.Application.Features.Courses.Dtos;
using SchoolMS.Application.Features.Courses.Queries.GetCourseById;
using SchoolMS.Application.Features.Courses.Queries.GetCourses;
using SchoolMS.Contracts.Courses;

namespace SchoolMS.Api.Endpoints;

public static class CourseEndpoints
{
    public static void MapCourseEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/courses").WithTags("Courses");

        group.MapGet("", GetCourses)
            .WithSummary("Get list of courses")
            .WithDescription("Returns a paginated list of courses optionally filtered by department or search term.")
            .Produces<CursorResult<CourseDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", GetCourseById)
            .WithSummary("Get course by ID")
            .WithDescription("Returns a specific course using its unique identifier.")
            .Produces<CourseDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("", CreateCourse)
            .WithSummary("Create a new course")
            .WithDescription("Creates a new course record in the system.")
            .Accepts<CreateCourseRequest>("application/json")
            .Produces<CourseDto>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPatch("/{id:guid}", UpdateCourse)
            .WithSummary("Update an existing course")
            .WithDescription("Updates an existing course using its ID.")
            .Accepts<UpdateCourseRequest>("application/json")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetCourseById(Guid id, ISender sender)
    {
        var query = new GetCourseByIdQuery
        {
            Id = id
        };

        var result = await sender.Send(query);

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

        var result = await sender.Send(command);

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

        var result = await sender.Send(command);

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

        var result = await sender.Send(query);

        return result.Match(
           dto => Results.Ok(dto),
           errors => errors.ToProblem());

    }
}
