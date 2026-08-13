namespace myshop.BLL.DTOs.Category;

public record CategoryListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedTime { get; set; }
}