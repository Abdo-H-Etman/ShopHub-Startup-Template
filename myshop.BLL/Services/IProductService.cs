using myshop.BLL.DTOs;
using myshop.BLL.DTOs.Common;
using myshop.BLL.DTOs.Product;

namespace myshop.BLL.Services;

public interface IProductService
{
    Task<IEnumerable<ProductListDto>> GetAllAsync();
    Task<IEnumerable<ProductListDto>> GetArchivedAsync();
    Task<PagedResultDto<ProductListDto>> GetPagedAsync(int pageNumber, int pageSize, string? search, string? sort);
    Task<ProductListDto?> GetByIdAsync(int id);
    Task<UpdateProductDto?> GetByIdForUpdateAsync(int id);
    Task CreateAsync(CreateProductDto dto, ImageUpload? imageUpload, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateProductDto dto, ImageUpload? imageUpload, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id);
    Task RestoreAsync(int id);
}
