namespace myshop.BLL.DTOs;

public class ImageUpload
{
    public string FileName { get; init; } = string.Empty;
    public long Length { get; init; }
    public Stream Content { get; init; } = Stream.Null;
}