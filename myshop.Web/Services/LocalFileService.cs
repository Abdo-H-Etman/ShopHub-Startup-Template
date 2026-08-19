using Microsoft.AspNetCore.Http;
using myshop.BLL.Services;

namespace myshop.Web.Services;

public class LocalFileService : IFileService
{
    private static readonly string[] AllowedExtensions =
    [
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    ];

    private const long MaxFileSize = 2 * 1024 * 1024;

    private readonly IWebHostEnvironment _environment;

    public LocalFileService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveFileAsync(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            throw new ArgumentException("Please select an image.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!AllowedExtensions.Contains(extension))
        {
            throw new ArgumentException(
                "Invalid image format. Only JPG, JPEG, PNG, and WEBP images are allowed.");
        }

        if (file.Length > MaxFileSize)
        {
            throw new ArgumentException(
                "The image size must not exceed 2 MB.");
        }

        var uploadsFolder = Path.Combine(
            _environment.WebRootPath,
            "uploads",
            "products");

        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{extension}";

        var filePath = Path.Combine(uploadsFolder, fileName);

        await using var stream = new FileStream(
            filePath,
            FileMode.Create);

        await file.CopyToAsync(stream);

        return $"uploads/products/{fileName}";
    }

    public async Task<string> CopyFileAsync(string sourceRelativePath)
    {
        var sourcePath = Path.Combine(
            _environment.WebRootPath,
            sourceRelativePath);

        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException(
                $"Seed image was not found: {sourceRelativePath}");
        }

        var extension = Path.GetExtension(sourcePath).ToLowerInvariant();

        if (!AllowedExtensions.Contains(extension))
        {
            throw new ArgumentException(
                "Invalid image format. Only JPG, JPEG, PNG, and WEBP images are allowed.");
        }

        var uploadsFolder = Path.Combine(
            _environment.WebRootPath,
            "uploads",
            "products");

        Directory.CreateDirectory(uploadsFolder);

        var fileName = $"{Guid.NewGuid()}{extension}";

        var destinationPath = Path.Combine(
            uploadsFolder,
            fileName);

        await using var sourceStream = new FileStream(
            sourcePath,
            FileMode.Open,
            FileAccess.Read);

        await using var destinationStream = new FileStream(
            destinationPath,
            FileMode.Create);

        await sourceStream.CopyToAsync(destinationStream);

        return $"uploads/products/{fileName}";
    }

    public void DeleteFile(string? filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return;

        var physicalPath = Path.Combine(
            _environment.WebRootPath,
            filePath.TrimStart('/')
                       .Replace('/', Path.DirectorySeparatorChar));

        if (File.Exists(physicalPath))
        {
            File.Delete(physicalPath);
        }
    }
}