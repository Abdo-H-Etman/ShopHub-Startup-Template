using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using myshop.BLL.DTOs;
using myshop.BLL.DTOs.Common;
using myshop.BLL.DTOs.Product;
using myshop.DAL.Services;
using myshop.Entities.Models;
using Repositories.Interfaces;

namespace myshop.BLL.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly IImageValidationService _imageValidationService;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService, IImageValidationService imageValidationService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileService = fileService;
        _imageValidationService = imageValidationService;
    }

    public async Task<IEnumerable<ProductListDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetAllAsync(q => q.Include(x => x.Category).Include(x => x.Reviews), cancellationToken: cancellationToken);
        return _mapper.Map<IEnumerable<ProductListDto>>(products);
    }

    public async Task<IEnumerable<ProductListDto>> GetArchivedAsync(CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetAllAsync(
            q => q.Where(p => p.IsDeleted).Include(x => x.Category).Include(x => x.Reviews),
            ignoreQueryFilters: true,
            cancellationToken: cancellationToken
        );

        return _mapper.Map<IEnumerable<ProductListDto>>(products);
    }

    public async Task<PagedResultDto<ProductListDto>> GetPagedAsync(int pageNumber, int pageSize, string? search, string? sort, CancellationToken cancellationToken = default)
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
            orderBy,
            cancellationToken: cancellationToken
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

    public async Task<ProductListDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.FirstOrDefaultAsync(
            p => p.Id == id,
            q => q.Include(x => x.Category).Include(x => x.Reviews),
            cancellationToken: cancellationToken
        );

        if (product is null) return null;

        return _mapper.Map<ProductListDto>(product);
    }

    public async Task<UpdateProductDto?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.FirstOrDefaultAsync(
            p => p.Id == id,
            q => q.Include(x => x.Category),
            cancellationToken: cancellationToken
        );

        if (product is null) return null;

        return _mapper.Map<UpdateProductDto>(product);
    }

    public async Task CreateAsync(CreateProductDto dto, ImageUpload? image, CancellationToken cancellationToken = default)
    {
        if (image is not null)
        {
            var validationResult = _imageValidationService.IsValid(image.FileName, image.Length);

            if (!validationResult.isValid)
            {
                throw new ArgumentException(validationResult.errorMessage);
            }

            dto.Img = await _fileService.SaveFileAsync(image.FileName, image.Content, cancellationToken);
        }

        var product = _mapper.Map<Product>(dto);

        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, UpdateProductDto dto, ImageUpload? image, CancellationToken cancellationToken = default)
    {
        var existingProduct = await _unitOfWork.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (existingProduct is null)
        {
            throw new InvalidOperationException("Product not found.");
        }

        if (image is not null)
        {
            await _fileService.DeleteAsync(existingProduct.Img, cancellationToken);

            var validationResult = _imageValidationService.IsValid(image.FileName, image.Length);

            if (!validationResult.isValid)
            {
                throw new ArgumentException(validationResult.errorMessage);
            }
            dto.Img = await _fileService.SaveFileAsync(image.FileName, image.Content, cancellationToken);
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

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken: cancellationToken);
        if (product is null)
        {
            throw new InvalidOperationException("Product not found.");
        }

        await _unitOfWork.Products.DeleteAsync(id, cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);
    }

    public async Task RestoreAsync(int id, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Products.RestoreAsync(id, cancellationToken: cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);
    }
}
