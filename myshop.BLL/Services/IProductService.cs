using Microsoft.AspNetCore.Http;
using myshop.BLL.DTOs.Product;

namespace myshop.BLL.Services;

public interface IProductService
{
    Task<IEnumerable<ProductListDto>> GetAllAsync();
    Task<ProductListDto?> GetByIdAsync(int id);
    Task<UpdateProductDto?> GetByIdForUpdateAsync(int id);
    Task CreateAsync(CreateProductDto dto, IFormFile? file, string webRootPath);
    Task UpdateAsync(int id, UpdateProductDto dto, IFormFile? file, string webRootPath);
    Task DeleteAsync(int id, string webRootPath);
}
