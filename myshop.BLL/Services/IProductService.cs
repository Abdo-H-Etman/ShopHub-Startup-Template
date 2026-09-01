using myshop.BLL.DTOs;
using myshop.BLL.DTOs.Common;
using myshop.BLL.DTOs.Product;

namespace myshop.BLL.Services;

public interface IProductService
{
    Task<IEnumerable<ProductListDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductListDto>> GetArchivedAsync(CancellationToken cancellationToken = default);
    Task<PagedResultDto<ProductListDto>> GetPagedAsync(int pageNumber, int pageSize, string? search, string? sort, CancellationToken cancellationToken = default);
    Task<ProductListDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UpdateProductDto?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken = default);
    Task CreateAsync(CreateProductDto dto, ImageUpload? imageUpload, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateProductDto dto, ImageUpload? imageUpload, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task RestoreAsync(int id, CancellationToken cancellationToken = default);
}
