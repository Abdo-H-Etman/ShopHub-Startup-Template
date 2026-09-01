using Microsoft.AspNetCore.Hosting;
using myshop.DAL.Services;

namespace myshop.BLL.Services;

public class FilePathsService : IFilePathsService
{
    private readonly IWebHostEnvironment _environment;

    public FilePathsService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public string GetSaveFilePath()
    {
        return Path.Combine(
            _environment.WebRootPath,
            "uploads",
            "products");
    }
}