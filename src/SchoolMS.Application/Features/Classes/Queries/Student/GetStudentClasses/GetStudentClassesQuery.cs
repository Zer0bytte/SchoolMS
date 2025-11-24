using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Classes.Dtos;

namespace SchoolMS.Application.Features.Classes.Queries.Student.GetStudentClasses;

public class GetStudentClassesQuery : CursorQuery, IRequest<Result<CursorResult<ClassDto>>>;
