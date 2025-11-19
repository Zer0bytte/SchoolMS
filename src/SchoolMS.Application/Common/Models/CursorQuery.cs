using System.ComponentModel.DataAnnotations;

namespace SchoolMS.Application.Common.Models;

public class CursorQuery
{
    public string? Cursor { get; set; }
    [Range(1, 100)]
    public int Limit { get; set; }
}
