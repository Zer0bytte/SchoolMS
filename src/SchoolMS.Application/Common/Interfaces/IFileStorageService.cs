namespace SchoolMS.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string?> SaveFileAsync(FileData file, string folderPath);
}