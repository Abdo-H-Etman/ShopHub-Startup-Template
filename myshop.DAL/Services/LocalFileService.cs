using Microsoft.AspNetCore.Http;

namespace myshop.DAL.Services;

public class LocalFileService : IFileService
{
    private readonly IFilePathsService _filePathsService;

    public LocalFileService(IFilePathsService filePathsService)
    {
        _filePathsService = filePathsService;
    }

    public async Task<string> SaveFileAsync(string fileName, Stream fileStream, CancellationToken cancellationToken = default)
    {

        var extension = Path.GetExtension(fileName).ToLowerInvariant();

        var uploadsFolder = _filePathsService.GetSaveFilePath();

        Directory.CreateDirectory(uploadsFolder);

        var GeneratedFileName = $"{Guid.NewGuid()}{extension}";

        var filePath = Path.Combine(uploadsFolder, GeneratedFileName);

        await using var stream = new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None);

        await fileStream.CopyToAsync(stream, cancellationToken);

        return $"uploads/products/{GeneratedFileName}";
    }

    public async Task<string> CopyFileAsync(string sourceRelativePath, CancellationToken cancellationToken = default)
    {
        var uploadsFolder = _filePathsService.GetSaveFilePath();

        Directory.CreateDirectory(uploadsFolder);

        var extension = Path.GetExtension(sourceRelativePath).ToLowerInvariant();

        var fileName = $"{Guid.NewGuid()}{extension}";

        var destinationPath = Path.Combine(
            uploadsFolder,
            fileName);

        await using var sourceStream = new FileStream(
            sourceRelativePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read);

        await using var destinationStream = new FileStream(
            destinationPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None);

        await sourceStream.CopyToAsync(destinationStream);

        return $"uploads/products/{fileName}";
    }

    public async Task DeleteAsync(string? filePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return;

        var folderPath = _filePathsService.GetSaveFilePath();

        var fileName = Path.GetFileName(filePath);

        var fullPath = Path.Combine(folderPath, fileName);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        await Task.CompletedTask;
    }
}