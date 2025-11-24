using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SchoolMS.Application.Common.Interfaces;

namespace SchoolMS.Infrastructure.FileSaving;

public sealed class FileStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContext) : IFileStorageService
{
    private string GetBaseUrl()
    {
        var request = httpContext.HttpContext?.Request;

        if (request == null)
            return string.Empty;

        return $"{request.Scheme}://{request.Host}";
    }

    public async Task<string?> SaveFileAsync(FileData file, string folderPath)
    {
        try
        {
            string root = Path.Combine(env.WebRootPath, "uploads");

            string fullFolderPath = Path.Combine(root, folderPath);

            Directory.CreateDirectory(fullFolderPath);

            string uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            string filePath = Path.Combine(fullFolderPath, uniqueName);

            using (var outputStream = new FileStream(filePath, FileMode.Create))
            {
                file.Content.Seek(0, SeekOrigin.Begin);
                await file.Content.CopyToAsync(outputStream);
            }

            string baseUrl = GetBaseUrl();
            string relative = $"/uploads/{folderPath}/{uniqueName}".Replace("\\", "/");
            return $"{baseUrl}{relative}";
        }
        catch
        {
            return null;
        }
    }
}

