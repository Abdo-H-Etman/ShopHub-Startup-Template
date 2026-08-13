using myshop.BLL.DTOs.Category;
namespace myshop.BLL.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryListDto>> GetAllAsync();
    Task<CategoryListDto?> GetByIdAsync(int id);
    Task<UpdateCategoryDto?> GetByIdForUpdateAsync(int id);
    Task CreateAsync(CreateCategoryDto dto);
    Task UpdateAsync(UpdateCategoryDto dto);
    Task DeleteAsync(int id);
}
