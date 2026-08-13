using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using myshop.BLL.DTOs.Product;
using myshop.Entities.Models;
using Repositories.Interfaces;

namespace myshop.BLL.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductListDto>> GetAllAsync()
    {
        var products = await _unitOfWork.Products.GetAllAsync(q => q.Include(x => x.Category));
        return _mapper.Map<IEnumerable<ProductListDto>>(products);
    }

    public async Task<ProductListDto?> GetByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.FirstOrDefaultAsync(p => p.Id == id, q => q.Include(x => x.Category));
        if (product is null) return null;

        return _mapper.Map<ProductListDto>(product);
    }

    public async Task<UpdateProductDto?> GetByIdForUpdateAsync(int id)
    {
        var product = await _unitOfWork.Products.FirstOrDefaultAsync(p => p.Id == id, q => q.Include(x => x.Category));
        if (product is null) return null;

        return _mapper.Map<UpdateProductDto>(product);
    }
    public async Task CreateAsync(CreateProductDto dto, IFormFile? file, string webRootPath)
    {
        if (file is not null)
        {
            var filename = Guid.NewGuid().ToString();
            var uploadPath = Path.Combine(webRootPath, "Images", "Products");
            Directory.CreateDirectory(uploadPath);
            var ext = Path.GetExtension(file.FileName);

            using var stream = new FileStream(Path.Combine(uploadPath, filename + ext), FileMode.Create);
            await file.CopyToAsync(stream);

            dto.Img = Path.Combine("Images", "Products", filename + ext).Replace('\\', '/');
        }

        var product = _mapper.Map<Product>(dto);
        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateProductDto dto, IFormFile? file, string webRootPath)
    {
        var existingProduct = await _unitOfWork.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (existingProduct is null)
        {
            throw new InvalidOperationException("Product not found.");
        }

        if (file is not null)
        {
            if (!string.IsNullOrWhiteSpace(existingProduct.Img))
            {
                var oldImagePath = Path.Combine(webRootPath, existingProduct.Img.TrimStart('~', '/').Replace('/', Path.DirectorySeparatorChar));
                if (File.Exists(oldImagePath))
                {
                    File.Delete(oldImagePath);
                }
            }

            var filename = Guid.NewGuid().ToString();
            var uploadPath = Path.Combine(webRootPath, "Images", "Products");
            Directory.CreateDirectory(uploadPath);
            var ext = Path.GetExtension(file.FileName);

            using var stream = new FileStream(Path.Combine(uploadPath, filename + ext), FileMode.Create);
            await file.CopyToAsync(stream);

            dto.Img = Path.Combine("Images", "Products", filename + ext).Replace('\\', '/');
        }
        else
        {
            dto.Img = existingProduct.Img;
        }

        dto.Id = id;
        _mapper.Map(dto, existingProduct);
        // existingProduct.Category = existingProduct.Category;
        await _unitOfWork.Products.UpdateAsync(existingProduct);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string webRootPath)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product is null)
        {
            throw new InvalidOperationException("Product not found.");
        }

        if (!string.IsNullOrWhiteSpace(product.Img))
        {
            var filePath = Path.Combine(webRootPath, product.Img.TrimStart('~', '/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        await _unitOfWork.Products.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
