namespace myshop.BLL.Services;

public class ImageValidationService : IImageValidationService
{
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

    private const long MaxFileSize = 2 * 1024 * 1024;

    public (bool isValid, string errorMessage) IsValid(string fileName, long fileSize)
    {
        var extension = Path.GetExtension(fileName);

        if (!AllowedExtensions.Contains(extension))
        {
            return (false, "Invalid image format. Only JPG, JPEG, PNG, and WEBP images are allowed.");
        }

        if (fileSize > MaxFileSize)
        {
            return (false, "The image size must not exceed 2 MB.");
        }

        return (true, string.Empty);
    }
}