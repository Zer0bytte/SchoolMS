namespace SchoolMS.Application.Common.Interfaces;

public sealed class FileData
{
    public string FileName { get; init; } = default!;
    public Stream Content { get; init; } = default!;
}
