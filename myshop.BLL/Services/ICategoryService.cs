using myshop.BLL.DTOs.Category;
namespace myshop.BLL.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryListDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CategoryListDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UpdateCategoryDto?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken = default);
    Task CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdateCategoryDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
