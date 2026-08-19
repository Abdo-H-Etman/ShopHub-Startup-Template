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
    private readonly IFileService _fileService;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileService = fileService;
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
    public async Task CreateAsync(CreateProductDto dto, IFormFile? file)
    {
        if (file is not null)
        {
            dto.Img = await _fileService.SaveFileAsync(file);
        }

        var product = _mapper.Map<Product>(dto);

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateProductDto dto, IFormFile? file)
    {
        var existingProduct = await _unitOfWork.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (existingProduct is null)
        {
            throw new InvalidOperationException("Product not found.");
        }

        if (file is not null)
        {
            _fileService.DeleteFile(existingProduct.Img);

            dto.Img = await _fileService.SaveFileAsync(file);
        }
        else
        {
            dto.Img = existingProduct.Img;
        }

        dto.Id = id;
        _mapper.Map(dto, existingProduct);

        await _unitOfWork.Products.UpdateAsync(existingProduct);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product is null)
        {
            throw new InvalidOperationException("Product not found.");
        }

        _fileService.DeleteFile(product.Img);

        await _unitOfWork.Products.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
