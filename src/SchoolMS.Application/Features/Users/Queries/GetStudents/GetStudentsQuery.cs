using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Users.Dtos;

namespace SchoolMS.Application.Features.Users.Queries.GetStudents;

public class GetStudentsQuery : CursorQuery, IRequest<Result<CursorResult<StudentDto>>>;