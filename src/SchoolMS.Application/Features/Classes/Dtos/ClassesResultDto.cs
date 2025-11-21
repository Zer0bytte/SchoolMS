using SchoolMS.Application.Common.Models;

namespace SchoolMS.Application.Features.Classes.Dtos;

public class ClassesResultDto : CurosrResult
{
    public List<ClassDto> Items { get; set; }
}
