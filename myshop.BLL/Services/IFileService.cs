using Microsoft.AspNetCore.Http;

namespace myshop.BLL.Services;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file);
    Task<string> CopyFileAsync(string sourceRelativePath);
    void DeleteFile(string? filePath);
}