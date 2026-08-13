namespace myshop.BLL.DTOs.Category;

public record CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
