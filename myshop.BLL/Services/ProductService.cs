using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using myshop.BLL.DTOs.Common;
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
        var products = await _unitOfWork.Products.GetAllAsync(q => q.Include(x => x.Category).Include(x => x.Reviews));
        return _mapper.Map<IEnumerable<ProductListDto>>(products);
    }

    public async Task<IEnumerable<ProductListDto>> GetArchivedAsync()
    {
        var products = await _unitOfWork.Products.GetAllAsync(
            q => q.Where(p => p.IsDeleted).Include(x => x.Category).Include(x => x.Reviews),
            ignoreQueryFilters: true
        );

        return _mapper.Map<IEnumerable<ProductListDto>>(products);
    }

    public async Task<PagedResultDto<ProductListDto>> GetPagedAsync(int pageNumber, int pageSize, string? search, string? sort)
    {
        pageNumber = Math.Max(pageNumber, 1);
        pageSize = Math.Clamp(pageSize, 1, 50);

        search = search?.Trim();

        Expression<Func<Product, bool>>? predicate = null;

        if (!string.IsNullOrEmpty(search))
        {
            predicate = p => p.Name.Contains(search) || p.Description.Contains(search);
        }

        Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy =
            sort?.ToLower() switch
            {
                "namedesc" => q => q.OrderByDescending(p => p.Name),
                "priceasc" => q => q.OrderBy(p => p.Price),
                "pricedesc" => q => q.OrderByDescending(p => p.Price),
                _ => q => q.OrderBy(p => p.Name)
            };

        var (items, totalCount) = await _unitOfWork.Products.GetPagedAsync(
            pageNumber,
            pageSize,
            predicate,
            q => q.Include(p => p.Category).Include(p => p.Reviews),
            orderBy
        );

        var result = new PagedResultDto<ProductListDto>
        {
            Items = _mapper.Map<IEnumerable<ProductListDto>>(items),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return result;
    }

    public async Task<ProductListDto?> GetByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.FirstOrDefaultAsync(
            p => p.Id == id,
            q => q.Include(x => x.Category).Include(x => x.Reviews)
        );

        if (product is null) return null;

        return _mapper.Map<ProductListDto>(product);
    }

    public async Task<UpdateProductDto?> GetByIdForUpdateAsync(int id)
    {
        var product = await _unitOfWork.Products.FirstOrDefaultAsync(
            p => p.Id == id,
            q => q.Include(x => x.Category)
        );

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

        await _unitOfWork.Products.DeleteAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RestoreAsync(int id)
    {
        await _unitOfWork.Products.RestoreAsync(id);
        await _unitOfWork.SaveChangesAsync();
    }
}
