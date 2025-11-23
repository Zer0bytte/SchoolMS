using SchoolMS.Application.Common.Models;
using SchoolMS.Application.Features.Classes.Dtos;

namespace SchoolMS.Application.Features.Classes.Queries.GetTeacherClasses;

public class GetTeacherClassesQuery : CursorQuery, IRequest<Result<CursorResult<ClassDto>>>
{

}
