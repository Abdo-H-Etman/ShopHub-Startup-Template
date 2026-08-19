using Microsoft.AspNetCore.Http;
using myshop.BLL.DTOs.Product;

namespace myshop.BLL.Services;

public interface IProductService
{
    Task<IEnumerable<ProductListDto>> GetAllAsync();
    Task<ProductListDto?> GetByIdAsync(int id);
    Task<UpdateProductDto?> GetByIdForUpdateAsync(int id);
    Task CreateAsync(CreateProductDto dto, IFormFile? file);
    Task UpdateAsync(int id, UpdateProductDto dto, IFormFile? file);
    Task DeleteAsync(int id);
}
