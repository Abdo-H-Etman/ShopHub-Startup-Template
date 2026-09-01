namespace myshop.DAL.Services;

public interface IFileService
{
        Task<string> SaveFileAsync(string fileName, Stream fileStream,
                CancellationToken cancellationToken = default);
        Task<string> CopyFileAsync(string sourceRelativePath,
                CancellationToken cancellationToken = default);
        Task DeleteAsync(string? filePath, CancellationToken cancellationToken = default);
}