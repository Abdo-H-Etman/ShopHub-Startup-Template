namespace myshop.BLL.DTOs.Category;

public record UpdateCategoryDto : CreateCategoryDto
{
    public int Id { get; set; }
}